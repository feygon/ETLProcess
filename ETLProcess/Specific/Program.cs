#define Statement

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using ETLProcess.Specific;

using ETLProcess.Tests;
using ETLProcessFactory.Containers;
using ETLProcessFactory;
using ETLProcessFactory.Containers.AbstractClasses;
using ETLProcessFactory.Profiles;
using ETLProcessFactory.IO;
using ETLProcess.Specific.Boilerplate;
using ETLProcessFactory.Interfaces;
using ETLProcess.Specific.Documents;
using ETLProcessFactory.Containers.Members;

using String = System.String;
using AcctID = System.String;
using MemberID = System.String;
using System.Reflection;
using ETLProcessFactory.GP;

namespace ETLProcess {
    /// <summary>
    /// Enumeration of Implementation-specific Document Types
    /// </summary>
    public enum RecordType
    {
        /// <summary> Client's Statement file </summary>
        Statements,
        /// <summary> Client's Member File </summary>
        Members,
        /// <summary> Client's Balance Forward file </summary>
        BalancesForward,
        /// <summary> Catch-all error enumeration </summary>
        Error = -42
    }

    internal static class Program {
        private static ClientETLProcess client;
        
        private static void Main(string[] args)
        {
            try
            {
                Log.InitLog(args[0]);
                // Get customer rules defined in ETLProcessor definitions.
                Log.Write("Test");
#if Debug
                HashCodeTest.TestHashSystem();
#endif
                Log.Write("Test");


                string arg2 = "out.txt";
                if (args.Length >= 2) { arg2 = args[1] ?? "out.txt"; }
                client = new ClientETLProcess(args[0], arg2 ?? "out.txt");

                // input data from basic record types (Statement, Balance Forward, Member Files)
                client.PopulateRecords();

                // Enact client business rules
                List<OutputDoc> outputDocs = client.ProcessRecords(
                    out IEnumerable<DataRow> membersWithoutStatements
                    , out IEnumerable<DataRow> balancesWithoutStatements
                    , out IEnumerable<DataRow> statementsWithoutMembers);
                // Export SQL reports to LocalDB file.
                client.ExportReports();

                DelRet<bool> SqlReportCheck = client.GetCheck_SQL_Output(new object[] { client.TablesByType[typeof(Record_Statement)].First() });
                bool success = SqlReportCheck();
                

                // Report on these, other than returned outputDocs, probably by SQLBulkCopy to a new or proscribed table,
                //  or output to csv.
                
                // output data to client profiles
                client.XMLExport(outputDocs);

                //throw new Exception("Finish implementing ProcessDocs.");
                /**********************************/

                // only remove debug if completes.
                Log.Remove();
            } catch (Exception err) {
                string Message = err.Message;
                string StackTrace = err.StackTrace;
                string SourceApplication = err.Source;
                Exception inner = err.InnerException;
                var DataDictionary = err.Data;
                Log.Write(err.ToString());
                Console.Write(String.Format("Error:\n\n{0}", err.ToString()));
            } finally {
                CleanUp();
            }
        }

        private static void CleanUp() {
#if !DEBUG
            // Clean up any temp files
            if (Directory.Exists(IODirectory.TempLocation))
            {
                Directory.Delete(IODirectory.TempLocation, true);
            }
#endif
        }
    }
}