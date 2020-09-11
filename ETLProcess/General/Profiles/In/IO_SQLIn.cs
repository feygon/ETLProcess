using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

using ETLProcess.General.IO;
using ETLProcess.General.Interfaces;
using ETLProcess.General.Containers.AbstractClasses;

namespace ETLProcess.General.Profiles
{
    /// <summary>
    /// A program class for ETLProcesses that input from a sql query, and output xml.
    /// </summary>
    public class IO_SQLIn : SingletonProfile<IO_SQLIn>, IDisposable {
        /// <summary>
        /// This object's sql command
        /// </summary>
        public string SqlInCmd {
            get {
                if (firstRun) {
                    throw new Exception("First run instance should not be accessed. " +
                        "Try calling static instance SingletonProfile<IO_SQLIn>.Instance. member name");
                } else {
                    return sqlInCmd; 
                }
            }
        }
        private readonly string sqlInCmd = File.ReadAllText($@"{IOFiles.AssemblyDirectory}\{
                    Path.GetFileNameWithoutExtension(sqlFileName)}");

        private static string sqlFileName = "SQLInput.sql";

        /// <summary>
        /// This profile's selection sql command. (Extend class to overload/override?)
        /// </summary>
        public SqlCommand Query { 
            get => Query ?? 
                throw new Exception("Query is blank. Is this the disposable initial instance of this class? " +
                    "If so, please make sure the Init() method is in use for populating the Singleton Dictionary of Instances.");
            private set => Query = value;
        }

        /*****Constructors*****/
        /// <summary>
        /// Public parameterless constructor, for inheritance construction.
        /// </summary>
        public IO_SQLIn() : base(typeof(IO_FilesIn), null) { }

        /// <summary>
        /// Default Constructor (public parameterless constructor, for explicit cast only)
        /// </summary>

        /// <summary>
        /// Constructor, takes filename and parameters
        /// </summary>
        /// <param name="sqlFileName"></param>
        /// <param name="sqlCmdParams"></param>
        public IO_SQLIn(
            string sqlFileName = "SQLInput.sql"
            , (string pname, SqlDbType type)[] sqlCmdParams = null)
            : base(typeof(IO_FilesIn)
                  , new object[] { sqlFileName, sqlCmdParams })
        {
            IO_SQLIn.sqlFileName = sqlFileName;
            if (!firstRun)
            {
                Query = new SqlCommand(SqlInCmd, SQL.conn);
                foreach ((string pname, SqlDbType type) in sqlCmdParams)
                {
                    Query.Parameters.Add(pname, type);
                }
            }
        }

        /// <summary>
        /// A way to check input files for validity based upon a client-specific implementation.
        /// </summary>
        /// <param name="inputs">A delegate to check an array of filenames, returning bool for validity.</param>
        /// <returns></returns>
        public bool Check_Input(DelRet<bool, string[]> inputs) {
            return inputs(new string[] { sqlFileName });
        }

        /// <summary>
        /// A way to pre-check input files for validity based upon a client-specific implementation,
        ///     before instantiating this class.
        /// </summary>
        /// <param name="inputs">A delegate to check an array of filenames, returning bool for validity.</param>
        /// <param name="sqlFileName">The filename to check.</param>
        /// <returns></returns>
        public static bool PreCheck_Input(DelRet<bool, string[]> inputs, string sqlFileName = "SQLInput.sql")
        {
            IO_SQLIn sample = new IO_SQLIn();
            return inputs(new string[]{ sqlFileName });
        }
        /// <summary>
        /// A method to dispose of the initial instance. 
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