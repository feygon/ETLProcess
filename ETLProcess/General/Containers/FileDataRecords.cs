using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using ETLProcess.General.Interfaces;

using ETLProcess.General.IO;
using ETLProcess.General.Profiles;
using ETLProcess.General.Containers.Members;

using ETLProcess.General.Containers.AbstractClasses;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETLProcess.General.Containers
{
    
    /// <summary>
    /// Container class for lists of documents with various types of keys
    /// </summary>
    /// <typeparam name="TBasicRecord">The type of record to be populated into this dataTable.</typeparam>
    /// <typeparam name="TProfile">The client-specific ETLProcess class.</typeparam>
    public class FileDataRecords<TBasicRecord, TProfile>
        : DataTable, IGeneratesRecords where TBasicRecord
        : BasicRecord<TBasicRecord>, IRecord<TBasicRecord>, new()
        where TProfile : IC_CSVFileIn<IO_FilesIn>, new()
    {
        /// <summary>
        /// The number of times a redundant record was added.
        /// </summary>
        public int RedundantRecords { get; protected set; } = 0;

        /// <summary>
        /// The keystrings of all records added so far, from this class.
        /// </summary>
        public Dictionary<KeyStrings, bool> UniqueKeys_YN { get; } = new Dictionary<KeyStrings, bool>();

        /// <summary>
        /// This DataTable's unique counter index.
        /// </summary>
        protected static int ctr = 0;

        /// <summary>
        /// Is the key a unique key, or does it require an index to be unique?
        /// </summary>
        public bool unique;
        private readonly SampleColumnTypes columnInfo;
        internal BasicRecord<TBasicRecord> SampleBasicRecord = BasicRecord<TBasicRecord>.Sample;
        internal IRecord<TBasicRecord> SampleIRecord = BasicRecord<TBasicRecord>.Sample;

        /*****Constructors*****/

        /// <summary>
        /// A class to create a DataTable and link it to a DataSet, according to a constraint.
        /// </summary>
        /// <param name="source">Each Stringmap in the List of Stringmaps is a line of strings, with a string for a column. 
        /// <param name="constraint">Optional assembly of settings for foreign key constraints for this table, if any.</param>
        /// <br>The List of strings after this is the headers.</br></param>
        public FileDataRecords(
            HeaderSource<List<StringMap>, List<string>> source
            , ForeignKeyConstraintElements constraint = null) : base(typeof(TBasicRecord).Name)
        {
            // reflect a member of the class referred to by the TProfile generic class argument,
            //  whose interface has "SampleColumns" and whose constructor takes a single string,
            //  then get the property of "SampleColumns" from it.
            // This enables the decoupling of this class from the client-specific ETLProcess.
            var tProfile = Activator.CreateInstance(
                typeof(TProfile)
                , new object[] { (string)IO_FilesIn.InstanceDict[typeof(IO_FilesIn)].classOptions[0] });
            Dictionary<Type, SampleColumnTypes> SampleColumns = 
                ((IC_CSVFileIn<IO_FilesIn>)tProfile).SampleColumns;

            columnInfo = SampleColumns[typeof(TBasicRecord)];

            // keyColumns = ClientETLProcess.Instance.SampleColumns[typeof(TBasicRecord)];
            
            unique = SampleBasicRecord.keyIsUniqueIdentifier;

            List<StringMap> dataList = source.data; // list of dictionaries of data strings, keyed by column strings
            List<string> headerStrings = source.headers;

            var src = new Dictionary<KeyStrings, TBasicRecord>();
            // TO DO: Analyze for parallelism
            foreach (StringMap strMap in dataList)
            {
                // Add a new Keystrings, record pair to the DataTable.
                TBasicRecord record = ((IRecord<TBasicRecord>)SampleBasicRecord).Record(
                        strMap
                        , SampleColumns[typeof(TBasicRecord)]
                        , headerStrings);
                src.Add(
                    record.recordKey,
                    record
                    );
            }
            TableName = $"FilesIn_Table_{ typeof(TBasicRecord).Name}_{ IOFiles.PrepGuid}_{ctr}";
            SetColumns();
            SetRows(src);
            constraint.masterSet.Tables.Add(this);
            LinkTable_FK(constraint);
            ctr++; // TO DO: Needed or not?
        }

        /// <summary>
        /// Add a single TBasicRecord to the base dictionary.
        /// </summary>
        /// <param name="key">Key of the record</param>
        /// <param name="record">The record</param>
        protected void Add(KeyStrings key, TBasicRecord record)
        {
            bool found = UniqueKeys_YN.ContainsKey(key);
            if (found) { 
                RedundantRecords++;
                UniqueKeys_YN[key] = false;
            } else { 
                UniqueKeys_YN.Add(key, true);
            }
            DataRow row = NewRow(); // method constructs and adds returned record to table.
            foreach (var cell in record)
            {
                // TO DO: Correct type parsing errors (Date, for example).
                if (!Columns.Contains(cell.Key)) { Log.WriteException($"Column named \"{cell.Key}\" not present in the table."); }
                row.SetField(Columns[cell.Key], cell.Value);
            }
            Rows.Add(row); // Does this check against primary keys for uniqueness?
        }

        /// <summary>
        /// Create a DataRecords table from a SQL file.
        /// </summary>
        /// <param name="inputProfile"></param>
        /// <param name="constraint">Optional constraint assembly, 
        ///     for linking this table to a master DataSet during its construction.</param>
        public FileDataRecords(
            IO_SQLIn inputProfile
            , ForeignKeyConstraintElements constraint = null)
        {
            this.TableName = 
                "SQLIn_Table_" 
                + typeof(TBasicRecord).ToString() 
                + "_" + IOFiles.PrepGuid 
                + ctr.ToString();
            DataTable table = new DataTable(TableName);
            table = SQL.ExecuteBuiltCommandReturnQuery(inputProfile.Query, table);
            ctr++;
            
            this.Merge(table);
            LinkTable_FK(constraint);
        }
        
        /*****Overloads*****/

        /// <summary>
        /// Dereference a single record on its unique composite key. For GP use.
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
            foreach (var keyName in columnInfo.Keys)
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
        // Add All columns to the Table's columns.
        // Append key columns to the Primary Key DataColumn array.
        private void SetColumns() {
            List<DataColumn> primaryKeys = new List<DataColumn>();
            foreach (KeyValuePair<string, (Type colType, bool isKey)> kvp
                in ((IRecord<TBasicRecord>)SampleBasicRecord).columnTypes)
            {
                DataColumn col = new DataColumn(kvp.Key, kvp.Value.colType);
                Columns.Add(col);
                if (kvp.Value.isKey) {
                    primaryKeys.Add(col);
                }
            }
            if (!unique)
            {
                DataColumn indexCol = new DataColumn("index", typeof(int));
                indexCol.Unique = true;
                indexCol.AutoIncrement = true;
                indexCol.ReadOnly = true;
                Columns.Add(indexCol);
                primaryKeys.Add(indexCol);
            }
            PrimaryKey = primaryKeys.ToArray();
        }

        /// <summary>
        /// Add rows to base table and recordsAdded key-watcher.
        /// </summary>
        private void SetRows(Dictionary<KeyStrings, TBasicRecord> source) {
            foreach (KeyValuePair<KeyStrings, TBasicRecord> kvp in source)
            {
                Add(kvp.Key, kvp.Value);
            }
        }
    } // end class.
} // end namespace.