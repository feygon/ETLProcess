#define Statement

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using ETLProcess.Specific;
using static ETLProcess.Parse;
using ETLProcess.General.Containers;
using ETLProcess.General;
using ETLProcess.General.Containers.AbstractClasses;
using ETLProcess.General.Profiles;
using ETLProcess.General.IO;
using ETLProcess.Specific.Boilerplate;
using ETLProcess.General.Interfaces;

using String = System.String;
using AcctID = System.String;
using MemberID = System.String;
using System.Reflection;

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
        private static Log log = Log.Instance;
        private static void Main(string[] args)
        {
            try {
                Log.InitLog(args[0]);
                // Get customer rules defined in ETLProcessor definitions.
                client = new ClientETLProcess(args[0]);

                // input data from basic record types (Statement, Balance Forward, Member Files)
                client.PopulateRecords();

                // Enact client business rules
                List<IOutputDoc> docs = client.ProcessRecords();

                // output data to client profiles
                client.ExportRecords();
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
#if !Debug
            // Clean up any temp files
            if (Directory.Exists(IOFiles.TempLocation))
            {
                Directory.Delete(IOFiles.TempLocation, true);
            }
#endif
        }
    }
}