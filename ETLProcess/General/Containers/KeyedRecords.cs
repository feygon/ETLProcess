using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Text;
using ETLProcess.General.Containers;
using ETLProcess.General.Interfaces;


namespace ETLProcess.General.Containers
{
    /// <summary>
    /// Container class for lists of documents with various types of keys
    /// </summary>
    /// <typeparam name="TBasicRecord"></typeparam>
    internal class KeyedRecords<TBasicRecord> 
        : Dictionary<KeyStrings, List<TBasicRecord>> where TBasicRecord
        : BasicRecord, IRecord<TBasicRecord>, new()
    {
        readonly TBasicRecord sample = new TBasicRecord();
        public bool unique;
        private Guid guid;
        private static Dictionary<Type, List<string>> keyColumns;
        public DataTable table;

        /// <summary>
        /// A class to contain data whether it is primary or composite keyed,
        ///     either redundantly or uniquely.
        ///     <br>TO DO: Could be more general purpose with more explicit interfaces,
        ///         by promising certain generic types for allBasicDocs and headers</br>
        /// </summary>
        /// <param name="source">Each Stringmap in the List of Stringmaps is a line of strings, with a string for a column. 
        /// <param name="guid">The unique id for this ETLProcess, from PreP.</param>
        /// <param name="process">The client-specific ETLProcess.</param>
        /// <param name="constraint">Optional: Elements needed to add a foreign key constraint to this object's table. </param>
        /// <br>The List of strings after this is the headers.</br></param>
        public KeyedRecords(
            HeaderSource<List<StringMap>, List<string>> source
            , Guid guid
            , IETLP_Specific<IETLP> process
            , ForeignKeyConstraintElements constraint) : base()
        {
            keyColumns = process.keyColumns;
            this.guid = guid;
            
            try
            {
                unique = sample.keyIsUniqueIdentifier;
            } catch {
                throw new Exception("Bad sample or unknown exception.");
            }

            // Build a Dictionary of TBasicRecords from a selection of instantiated records,
            //  on the key for each record.
            var unindexed = source.data.Select((line) =>
            {
                TBasicRecord record = sample.Record(line, source.headers);
                return new KeyValuePair<KeyStrings, TBasicRecord>(record.recordKey, record);
            }).ToList();

            foreach (KeyValuePair<KeyStrings, TBasicRecord> record in unindexed)
            {
                Add(record.Key, record.Value);
            }
            table = GetTable();

            if (constraint != null)
            {
                try
                {
                    setFKConstraint(constraint);
                    constraint.masterSet.Tables.Add(table);
                    if (constraint.primaryFKColumns != null)
                    {
                        setFKConstraint(constraint);
                    }
                }
                catch (Exception err)
                {
                    throw new System.Exception("Bad Foreign Key Constraint design or unknown exception: ", err);
                }
            }
        }

        // Set the prepared constraint.
        private void setFKConstraint(ForeignKeyConstraintElements constraint)
        {
            if (constraint.masterSet != null 
                && constraint.primaryFKColumns != null 
                && constraint.childFKColumnNames != null)
            {
                DataColumn[] childColumns = new DataColumn[constraint.primaryFKColumns.Length];
                for (int col = 0; col <= constraint.childFKColumnNames.Length; col++)
                {
                    childColumns[col] = table.Columns[constraint.childFKColumnNames[col]];
                }
                constraint.masterSet.Relations.Add(constraint.primaryFKColumns, childColumns);
            }
        }

        /// <summary>
        /// Add a single TBasicRecord to the base dictionary.
        /// </summary>
        /// <param name="key">Key of the record</param>
        /// <param name="record">The record</param>
        public void Add(KeyStrings key, TBasicRecord record)
        {
            bool success = base.TryGetValue(key, out List<TBasicRecord> ret);
            if (success) { 
                ret.Add(record);
            } else {
                base.Add(key, new TBasicRecord[] { record }.ToList());
            }
        }

        /// <summary>
        /// Dereference a single record on its unique composite key.
        /// </summary>
        /// <param name="recordKey">The unique key</param>
        /// <param name="ret">The return object.</param>
        /// <returns></returns>
        public bool TryGetValue (
            KeyStrings recordKey, out TBasicRecord ret)
        {
            if (!this.unique) throw new WarningException("Getting only first element of non-unique set.");

            bool success = base.TryGetValue(recordKey, out List<TBasicRecord> tryGot);
            if (success)
            {
                ret = tryGot.FirstOrDefault();
            } else { 
                ret = null;
            }
            return success;
        }

        /// <summary>
        /// Get a DataTable representation of the KeyedRecords dictionary.
        /// </summary>
        /// <returns></returns>
        private DataTable GetTable()
        {
            var sample = new TBasicRecord();
            var tableName = sample.GetChildType().ToString();
            var table = new DataTable(tableName);
            
            // add columns
            foreach (string column in sample.headers)
            {
                // get final data types from child records in a dictionary of sorts.
                table.Columns.Add(new DataColumn(column, sample.columnTypes[column]));
            }
            table.Columns.Add("indexNum" + guid, typeof(int));

            // set key columns
            var keyColumns = new List<DataColumn>();
            foreach (string key in sample.recordKey)
            {
                keyColumns.Add(table.Columns[key]);
            }
            keyColumns.Add(table.Columns["indexNum" + guid]);

            // set Primary Key based on keyColumns.
            table.PrimaryKey = keyColumns.ToArray();

            // add rows
#if !DEBUG
            var rows = from recordList in this
            select new {
                list = recordList.Key,
                instance = ((from records in recordList.Value select records
                            )).ToList()
            };
            foreach (var x in rows) {
                table.Rows.Add(x);
            }
#else
            // this ^^ does the same as this vv
            foreach (KeyValuePair<KeyStrings, List<TBasicRecord>> recordList in this)
            {
                for (int i=0; i < recordList.Value.Count; i++)
                {
                    DataRow row = table.NewRow();
                    foreach (string header in recordList.Value[i].headers)
                    {
                        row[header] = recordList.Value[i][header];
                    }
                }
            }
#endif
            return table;
        }
    } // end class.
} // end namespace.