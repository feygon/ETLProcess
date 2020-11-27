using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using ETLProcessFactory.Containers;
using ETLProcessFactory.Containers.AbstractClasses;
using ETLProcessFactory.Containers.Dictionaries;
using ETLProcessFactory.Interfaces;
using ETLProcessFactory.IO;
using UniversalCoreLib;

namespace ETLProcessFactory.Profiles
{
    // see Linq to SQL: https://docs.microsoft.com/en-us/dotnet/api/system.data.linq.mapping?view=netframework-4.8
    /// <summary>
    /// A program class for ETLProcesses that output to SQL with a sql query.
    /// </summary>
    public class Out_SQLBulkProfile<TTable> : 
        SingletonProfile<Out_SQLBulkProfile<TTable>>, IDisposable 
        where TTable : DataTable // So it can be used by the SQLBulkCopy
    {
        private static TTable dataTable = null;
        private static SqlBulkCopy _sqlBulkCopy = null;
        public SqlBulkCopy sqlBulkCopy
        { // TO DO: Implement w/ type conversion?
            get { 
                if (_sqlBulkCopy == null) { _sqlBulkCopy = new SqlBulkCopy(SQL.Conn.ConnectionString, copyOptions: 0); }
                return _sqlBulkCopy;
            }
            private set { _sqlBulkCopy = value; }
        } // See SqlBulkCopy.WriteToServer Method https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlbulkcopy.writetoserver?view=dotnet-plat-ext-3.1#System_Data_SqlClient_SqlBulkCopy_WriteToServer_System_Data_DataTable_
        
        private static string destinationTableName;

        /// <summary>
        /// Public parameterless constructor. For init only. May be initted before T is populated.
        /// </summary>
        public Out_SQLBulkProfile(TTable table) : base(typeof(Out_SQLBulkProfile<TTable>), new object[] { string.Format($"ETLProcessReport_{typeof(TTable).Name}"), SqlBulkCopyOptions.Default }) {
            if (!firstRun) { Log.WriteException(string.Format($"Error! " +
                $"Used initialization constructor for IO_SQLOut<{typeof(TTable).Name}> after first run! " +
                $"This constructor is only for use by the decoupled init method!")); }
            destinationTableName = string.Format($"ETLProcessReport_{typeof(TTable).Name}");
            dataTable = table;
        }

        /// <summary>
        /// Public constructor with parameters. Run this only *after* table T is populated.
        /// </summary>
        /// <param name="table">Which dataTable is being output?</param>
        /// <param name="tableName">What is the name of the destination table in the SQL database?
        /// Default -- ETLProcessReport_**typeof(T).Name**</param>
        /// <param name="sqlBulkCopyOptions"></param>
        public Out_SQLBulkProfile(
            string tableName
            , SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default) : 
                base(typeof(Out_SQLBulkProfile<TTable>), new object[]{
                    tableName, sqlBulkCopyOptions 
                }) 
        {
            Out_SQLBulkProfile<TTable>.destinationTableName = tableName;
        }

        /// <summary>
        /// Run the sql command and export using passed SQLBulkCopy adapter.
        /// </summary>
        public void Export(DelRet<bool> tableCheck)
        {
            Log.Write(string.Format($"Exporting table {destinationTableName} by SQLBulkCopy"));
            
            if (dataTable == null) { Log.WriteException("Using firstRun of SingletonProfile or unknown erroneous null dataTable."); }

            bool success = tableCheck();//reflect to ClientETLProcess.GetCheck_SQL_Output?;
            if (!success)
            {
                Log.WriteException("Table for SQL Report failed check.");
            }
            sqlBulkCopy.DestinationTableName = destinationTableName;
            GetColumnMappings();

            try
            {
                sqlBulkCopy.WriteToServer(dataTable);
            } catch (SqlException err) {
#if DEBUG
                Log.Write(string.Format($"Updated identical row in test LocalDB: {err.Message}, {err.Number}"));
#else
                Log.WriteException("Updated identical row: ", err);
#endif
            }


            Log.Write(string.Format($"SQL REPORT: Wrote dataTable {dataTable.TableName} to sql table {destinationTableName}."));
        }

        private void GetColumnMappings()
        {
            Type type = typeof(TTable).GetGenericArguments()[0];
            object obj = Activator.CreateInstance(type);
            var info = type.GetProperty("ColumnTypes");
            var tableHeaders = (TableHeaders)info.GetValue(obj);
            foreach (var col in tableHeaders)
            {
                sqlBulkCopy.ColumnMappings.Add(col.Key, col.Key);
            }
        }

        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public void Dispose() {
            if (firstRun) {
                disposable.Dispose();
                GC.SuppressFinalize(this);
            } else {
                Log.WriteException("Dispose called on non-initial singleton instance.");
            }
        }
    }
}