using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ETLProcess.General.Containers;
using ETLProcess.General.Interfaces;
using System.CodeDom;
using System.Reflection;
using SampleColumnTypes = System.Collections.Generic.Dictionary<string, System.Type>;
using ETLProcess.General.Containers.AbstractClasses;
using ETLProcess.General.Containers.Members;
using ETLProcess.General.Profiles;

namespace ETLProcess.General.Containers
{
    /// <summary>
    /// Container class for lists of documents with various types of keys
    /// </summary>
    /// <typeparam name="TBasicRecord"></typeparam>
    public class DataRecords<TBasicRecord>
        : DataTable, IGeneratesRecords where TBasicRecord
        : BasicRecord<TBasicRecord>, IRecord<TBasicRecord>, new()
    {

        /// <summary>
        /// The number of times a redundant record was added.
        /// </summary>
        public int redundantRecords { get; protected set; } = 0;
        /// <summary>
        /// The keystrings of all records added so far, from this class.
        /// </summary>
        public Dictionary<KeyStrings, bool> uniqueKeys_YN { get; } = new Dictionary<KeyStrings, bool>();

        /// <summary>
        /// This DataTable's unique counter index.
        /// </summary>
        protected static int ctr = 0;

        /// <summary>
        /// Is the key a unique key, or does it require an index to be unique?
        /// </summary>
        public bool unique;
        private readonly SampleColumnTypes keyColumns;
        internal BasicRecord<TBasicRecord> SampleBasicRecord = BasicRecord<TBasicRecord>.Sample;
        internal IRecord<TBasicRecord> SampleIRecord = BasicRecord<TBasicRecord>.Sample;

        /*****Constructors*****/

        /// <summary>
        /// A class to contain data whether it is primary or composite keyed,
        ///     either redundantly or uniquely.
        ///     <br>TO DO: Could be more general purpose with more explicit interfaces,
        ///         by promising certain generic types for allBasicDocs and headers</br>
        /// </summary>
        /// <param name="source">Each Stringmap in the List of Stringmaps is a line of strings, with a string for a column. 
        /// <param name="process">The client-specific ETLProcess.</param>
        /// <param name="constraint">Optional assembly of settings for foreign key constraints for this table, if any.</param>
        /// <br>The List of strings after this is the headers.</br></param>
        public DataRecords(
            HeaderSource<List<StringMap>, List<string>> source
            , IETLP_Specific<FilesIn_XMLOut> process
            , ForeignKeyConstraintElements constraint = null) : base(typeof(TBasicRecord).Name)
        {
            keyColumns = process.SampleColumns[typeof(TBasicRecord)];
            
            unique = SampleBasicRecord.keyIsUniqueIdentifier;

            List<StringMap> dataList = source.data; // list of dictionaries of data strings, keyed by column strings
            List<string> headerStrings = source.headers;

            var src = new Dictionary<KeyStrings, TBasicRecord>();
            foreach (StringMap strMap in dataList)
            {
                src.Add(
                    SampleBasicRecord.recordKey
                    , ((IRecord<TBasicRecord>)SampleBasicRecord).Record(strMap, headerStrings));
            }

            SetColumns();
            SetRows(src);

            LinkTable_FK(constraint);
        }



        /// <summary>
        /// Add a single TBasicRecord to the base dictionary.
        /// </summary>
        /// <param name="key">Key of the record</param>
        /// <param name="record">The record</param>
        public void Add(KeyStrings key, TBasicRecord record)
        {
            bool found = uniqueKeys_YN.ContainsKey(key);
            if (found) { 
                redundantRecords++;
                uniqueKeys_YN[key] = false;
            } else { 
                uniqueKeys_YN.Add(key, true);
            }
            DataRow row = NewRow(); // method constructs and adds returned record to table.
            foreach (var cell in record)
            {
                // This needs to convert the string to its new type.
                switch (keyColumns[cell.Key].Name)
                {
                    case "System.String":
                        row[cell.Key] = record[cell.Value];
                        break;
                    case "System.Int32":
                    case "System.Int64":
                        row[cell.Key] = Parse.IntParse(record[cell.Value]);
                        break;
                    case "System.Decimal":
                        row[cell.Key] = Parse.DecimalParse(record[cell.Value]);
                        break;
                    case "System.Float":
                        row[cell.Key] = Parse.FloatParse(record[cell.Value]);
                        break;
                    case "ETLProcess.General.Containers.Members.Date":
                        row[cell.Key] = Date.Parse(record[cell.Value]);
                        break;
                    case "ETLProcess.General.Containers.Members.Address":
                        throw new NotImplementedException("Multi-field address parsing not pre-implemented. Please see Address class for simple extension options, or simply use strings.");
                    default:
                        throw new NotImplementedException("Type not implemented in enum.");
                }
            }
        }

        /// <summary>
        /// Create a DataRecords table from a SQL file.
        /// </summary>
        /// <param name="inputProfile"></param>
        /// <param name="constraint">Optional constraint assembly, 
        ///     for linking this table to a master DataSet during its construction.</param>
        public DataRecords(
            SQLIn_XMLOut inputProfile
            , ForeignKeyConstraintElements constraint = null)
        {
            DataTable table = new DataTable("SQLIn_Table_" + IOProfile<SQLIn_XMLOut>.PrepGuid + ctr.ToString());
            table = SQL.ExecuteBuiltCommandReturnQuery(inputProfile.Query, table);
            ctr++;

            this.Merge(table);
            LinkTable_FK(constraint);
        }
        
        /*****Overloads*****/

        /// <summary>
        /// Dereference a single record on its unique composite key.
        /// </summary>
        /// <param name="recordKey">The unique key</param>
        /// <param name="ret">The return object.</param>
        /// <returns></returns>
        public bool TrySelectValue (
            KeyStrings recordKey, out DataRow ret)
        {
            if (!this.unique) throw new WarningException("Getting only first element of non-unique set.");

            DataRow[] filteredSelection = recordKey.Filter(this);
            bool success = filteredSelection.Length > 0;
            if (success)
            {
                try
                {
                    ret = filteredSelection[0] ??= null;
                } catch (NullReferenceException) {
                    ret = null; 
                }
            } else { 
                ret = null;
            }
            return success;
        }

        /*****Promises*****/

        /// <summary>
        /// Index the table according to the primary key in the data.
        /// </summary>
        public void IndexTable()
        {
            foreach (var keyName in keyColumns.Keys)
            {
                PrimaryKey.Append(Columns[keyName]);
            }
        }

        /// <summary>
        /// Link a dataTable to another, according to a given constraint.
        /// </summary>
        /// <returns></returns>
        public void LinkTable_FK(
            ForeignKeyConstraintElements constraint = null)
        {
            constraint?.SetFKConstraint(this);
        }

        /*****Private Methods*****/
        private void SetColumns() {
            foreach (KeyValuePair<string, Type> kvp in ((IRecord<TBasicRecord>)SampleBasicRecord).columnTypes)
            {
                Columns.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Add rows to base table and recordsAdded key-watcher.
        /// </summary>
        private void SetRows(Dictionary<KeyStrings, TBasicRecord> source) {
            foreach (KeyValuePair<KeyStrings, TBasicRecord> kvp in source)
            {
                Add(kvp.Key, kvp.Value);
                uniqueKeys_YN.Add(kvp.Key, true);
            }
        }
    } // end class.
} // end namespace.