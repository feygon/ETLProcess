using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using ETLProcess.General.Interfaces;

using ETLProcess.General.IO;
using ETLProcess.General.Profiles;

using ETLProcess.General.Containers.AbstractClasses;
using System.ComponentModel.DataAnnotations.Schema;
using HashInt = System.Int32;
using IndexInt = System.Int32;


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
        where TProfile : IInputFileType_CSV<IO_FilesIn>, new()
    {
        /// <summary>
        /// Long-form name, including unique process identifier, for this table.
        /// </summary>
        public string longName;

        /// <summary>
        /// This DataTable's unique counter index.
        /// </summary>
        protected static int ctr = 0;

        /// <summary>
        /// Is the key a unique key, or does it require an index to be unique?
        /// </summary>
        public bool unique;
        private readonly TableHeaders columnInfo;
        internal BasicRecord<TBasicRecord> SampleBasicRecord = BasicRecord<TBasicRecord>.Sample;
        internal IRecord<TBasicRecord> SampleIRecord = BasicRecord<TBasicRecord>.Sample;

        /*****Constructors*****/

        /// <summary>
        /// A class to create a DataTable and link it to a DataSet, according to a constraint.
        /// </summary>
        /// <param name="source">Each Stringmap in the List of Stringmaps is a line of strings, with a string for a column.</param>
        /// <param name="sampleColumns">A dictionary of column types from the client-specific implementation classes.</param>
        /// <param name="constraint">Optional assembly of settings for foreign key constraints for this table, if any.</param>
        /// <param name="longName">A short name for this table -- if null, will be the name of the record class in the table's rows.
        /// <br>The List of strings after this is the headers.</br></param>
        public FileDataRecords(
            HeaderSource<List<StringMap>, List<string>> source
            , Dictionary<Type, TableHeaders> sampleColumns
            , ForeignKeyConstraintElements constraint = null
            , string longName = null) : base(typeof(TBasicRecord).Name)
        {
            columnInfo = sampleColumns[typeof(TBasicRecord)];
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
                        , columnInfo
                        , headerStrings);
                src.Add(
                    record.recordKey,
                    record
                    );
            }
            TableName = $"FilesIn_Table_{ typeof(TBasicRecord).Name}_{ IOFiles.PrepGuid}_{ctr}";

            this.longName = longName ?? string.Format(
                $"FilesIn_Table_{SampleBasicRecord.GetChildType().Name}_{IOFiles.PrepGuid}_{ctr}");
            string index = null;
            if (ctr > 0) { index = string.Format($"_{ctr}"); }
            this.TableName = string.Format($"{SampleBasicRecord.GetChildType().Name}{index}");

            SetColumns();
            SetRows(src);
            constraint.masterSet.Tables.Add(this);
            LinkTable_FK(constraint);
            
            ctr++; // TO DO: Needed or not?
        }


        /// <summary>
        /// Create a DataRecords table from a SQL file.
        /// </summary>
        /// <param name="inputProfile"></param>
        /// <param name="constraint">Optional constraint assembly, 
        ///     for linking this table to a master DataSet during its construction.</param>
        /// <param name="longName">A short name for this table -- if null, will be the name of the record class in the table's rows.</param>
        public FileDataRecords(
            IO_SQLIn inputProfile
            , ForeignKeyConstraintElements constraint = null
            , string longName = null)
        {
            this.longName = longName ?? string.Format(
                $"SQLIn_Table_{SampleBasicRecord.GetChildType().Name}_{IOFiles.PrepGuid}_{ctr}");
            string index = null;
            if (ctr > 0) { index = string.Format($"_{ctr}"); }
            this.TableName = string.Format($"{SampleBasicRecord.GetChildType().Name}{index}");

            DataTable table = new DataTable(TableName);
            table = SQL.ExecuteBuiltCommandReturnQuery(inputProfile.Query, table);
            ctr++;
            
            this.Merge(table);
            LinkTable_FK(constraint);
        }

        /*****Overloads*****/

        /// <summary>
        /// Add a single TBasicRecord to the base dictionary.
        /// </summary>
        /// <param name="key">Key of the record</param>
        /// <param name="record">The record</param>
        protected void Add(KeyStrings key, TBasicRecord record)
        {
            DataRow row = NewRow(); // method constructs and adds returned record to table.
            foreach (var cell in record) {
                // TO DO: Correct type parsing errors (Date, for example).
                if (!Columns.Contains(cell.Key)) { Log.WriteException($"Column named \"{cell.Key}\" not present in the table."); }
                row.SetField(Columns[cell.Key], cell.Value);
            }
            Rows.Add(row); // Does this check against primary keys for uniqueness?
        }

        /*****Promises*****/
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
                DataColumn indexCol = new DataColumn("index", typeof(int)) {
                    Unique = true,
                    AutoIncrement = true,
                    ReadOnly = true
                };
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