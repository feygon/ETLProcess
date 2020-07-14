using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

using ETLProcess.General.IO;
using ETLProcess.General.Interfaces;
using ETLProcess.General.Containers.AbstractClasses;
using ETLProcess.General.Containers.Members;
using System.Data;
using ETLProcess.General.Containers;
using System.Data.SqlClient;

namespace ETLProcess.General.Profiles
{
    /// <summary>
    /// A program class for ETLProcesses that input from a sql query, and output xml.
    /// </summary>
    public class SQLIn_XMLOut : IOProfile<SQLIn_XMLOut>
    {
        /// <summary>
        /// This object's sql command
        /// </summary>
        public string SqlInCmd { get; }
        /// <summary>
        /// This profile's selection sql command. (Extend class to overload/override?)
        /// </summary>
        public SqlCommand Query { get; }

        /// <summary>
        /// This object's unique counter index.
        /// </summary>
        protected static int ctr = 0;

        /*****Constructors*****/

        /// <summary>
        /// Default Constructor (public parameterless constructor, for explicit cast only)
        /// </summary>
        public SQLIn_XMLOut() : base() { }

        /// <summary>
        /// Constructor, takes filename and parameters
        /// </summary>
        /// <param name="sqlFilename"></param>
        /// <param name="sqlCmdParams"></param>
        public SQLIn_XMLOut(
            string sqlFilename = "SQLInput.sql"
            , (string pname, SqlDbType type)[] sqlCmdParams = null)
        {
            SqlInCmd = File.ReadAllText($@"{Program.AssemblyDirectory}\{prepGuid}{
                    Path.GetFileNameWithoutExtension(sqlFilename)}");

            Query = new SqlCommand(SqlInCmd, SQL.conn);
            foreach ((string pname, SqlDbType type) in sqlCmdParams)
            {
                Query.Parameters.Add(pname, type);
            }

        }

        /*****Overrides not implemented*****/

        /// <summary> Sir Not Appearing in this Class</summary>
        /// <param name="checkFiles">And his minstrel.</param>
        public override bool Check_Files(DelRetArray<bool, string> checkFiles) {
            return checkFiles(files);
        }

        /// <summary> Sir Not Appearing in this Class</summary>
        /// <param name="filename"></param>
        public override void SetTempDir(string filename) {
            ZipFiles._TempLocation = $@"{Path.GetTempPath()}MetPrep_{PrepGuid}{Path.GetFileNameWithoutExtension(filename)}";
            int i = 0;
            while (Directory.Exists(ZipFiles._TempLocation))
            {
                ZipFiles._TempLocation += @"_" + i;
                i++;
            }
        }
    }
}