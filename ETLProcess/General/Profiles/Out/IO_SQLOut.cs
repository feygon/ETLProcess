using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ETLProcess.General.Containers.AbstractClasses;
using ETLProcess.General.IO;

namespace ETLProcess.General.Profiles
{
    // see Linq to SQL: https://docs.microsoft.com/en-us/dotnet/api/system.data.linq.mapping?view=netframework-4.8
    /// <summary>
    /// A program class for ETLProcesses that output to SQL with a sql query.
    /// </summary>
    public class IO_SQLOut : SingletonProfile<IO_SQLOut>, IDisposable
    {
        SqlConnection conn = SQL.Conn;
        SqlBulkCopy sqlBulkCopy
        { // TO DO: Implement w/ type conversion?
            get { throw new NotImplementedException(); }
        } // See SqlBulkCopy.WriteToServer Method https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlbulkcopy.writetoserver?view=dotnet-plat-ext-3.1#System_Data_SqlClient_SqlBulkCopy_WriteToServer_System_Data_DataTable_
        /// <summary>
        /// This object's built sql output command.
        /// </summary>
        public string SqlOutCmd { get { if (firstRun) { throw new Exception("First run instance should not be accessed." +
            " Please call IO_SQLOut.Init first, then the singleton.");
                } else { 
                    return sqlOutCmd;
                }
            } 
        }
        private string sqlOutCmd;

        /// <summary>
        /// Public parameterless constructor.
        /// </summary>
        public IO_SQLOut() : base(typeof(IO_SQLOut), null) {
            sqlOutCmd = "";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Public constructor with parameters.
        /// </summary>
        /// <param name="err"></param>
        public IO_SQLOut(Exception err) : base(typeof(IO_SQLOut), null) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a query command.
        /// </summary>
        public void SetCmd() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Run the sql command and export using passed SQLBulkCopy adapter.
        /// </summary>
        public void Export(SqlBulkCopy SBCAdapter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public void Dispose() {
            if (firstRun) {
                disposable.Dispose();
                GC.SuppressFinalize(this);
            } else {
                Log.WriteException("Dispose called on singleton or non-initial instance.");
            }
        }
    }
}