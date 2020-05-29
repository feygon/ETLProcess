#define Statement

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using BasicPreprocess.Specific;
using static BasicPreprocess.Parse;
using BasicPreprocess.General.Containers;
using BasicPreprocess.General;
using BasicPreprocess.General.IO;
using BasicPreprocess.Specific.Boilerplate;
using BasicPreprocess.General.Interfaces;

using String = System.String;
using AcctID = System.String;
using MemberID = System.String;

/// <summary>
/// Enumeration of Implementation-specific Document Types
/// </summary>
public enum DocType
{
    /// <summary>
    /// Client's Statement file
    /// </summary>
    Statements,
    /// <summary>
    /// Client's Member File
    /// </summary>
    Members,
    /// <summary>
    /// Client's Balance Forward file
    /// </summary>
    BalancesForward,
    /// <summary>
    /// Catch-all error enumeration.
    /// </summary>
    Error = -42
}

namespace BasicPreprocess
{
    using StringMap = Dictionary<string, string>;

    internal static class Program
    {
        //private const string _7ZipExecutable = @"C:\Program Files\7-Zip\7z.exe";
        //private static string _TempLocation;
        public static readonly Uri AssemblyPath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        public static readonly string AssemblyDirectory = Path.GetDirectoryName(AssemblyPath.LocalPath);
        public static Log Debug;
        private static ClientPreProcess client;
        private static XMLPreProcess_Takes_Files PreP;

        private static void Main(string[] args)
        {
            try
            {
                // Get customer rules defined in preprocessor definitions.

                PreP = new XMLPreProcess_Takes_Files(args[0]);
                client = new ClientPreProcess(PreP, args[1]);

                if (!PreP.Check_Files(client.CheckFiles_Delegate)) {
                    throw new Exception("Bad file count in zipfile.");
                } // specified check for implementation Statement/WelcomeLetters

                // input data to basic record types (Statement, Balance Forward, 
                client.PopulateDocs();

                client.ProcessDocs(client.docs);
                //throw new Exception("Finish implementing ProcessDocs.");
                /**********************************/

                // only remove debug if completes.
                Debug.Remove();
            }
            catch (Exception err)
            {
                string Message = err.Message;
                string StackTrace = err.StackTrace;
                string SourceApplication = err.Source;
                Exception inner = err.InnerException;
                var DataDictionary = err.Data;
                Console.Write(String.Format("Error:\n\n{0}", err.ToString()));
            }
            finally
            {
                CleanUp();
            }
        }

        // old main.
        ///// <summary>
        ///// Deprecated.
        ///// </summary>
        ///// <param name="args"></param>
        //private static void OldMain(string[] args)
        //{
        //    try
        //    {
        //        MetroEmail.InitClient();
        //        ZipFiles._TempLocation = $@"{Path.GetTempPath()}MetPrep_{DateTime.UtcNow.Ticks}{Path.GetFileNameWithoutExtension(args[0])}";
        //        while (Directory.Exists(ZipFiles._TempLocation))
        //        {
        //            ZipFiles._TempLocation += @"_2";
        //        }
        //        Debug = new Log($@"{AssemblyDirectory}\{DateTime.UtcNow.Ticks}{Path.GetFileNameWithoutExtension(args[0])}_Debug.log");
        //        // Begin the process
        //        Debug.Write("Preprocess Begun");
        //        // Handle 7zip extraction
        //        ZipFiles.Extract(args[0]); // the weeds.


        //        List<DocM691_Invoice> documents = MainProcessing(args[1]);
        //        XML.Export(args[0], documents, Debug); // the weeds.


        //        Debug.Remove();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Write(ex.Message);
        //        if (ex.InnerException != null)
        //        {
        //            Debug.Write(ex.InnerException.ToString());
        //        }
        //        Debug.Write("EXCEPTION: Process Closed IN ERROR");
        //    }
        //    finally
        //    {
        //        CleanUp();
        //    }
        //}

        // moved to ZipFiles.cs
        ///// <summary>
        ///// Uses 7Zip to extract the filePath to the _TempLocation. If the extract fails then it is assumed the file is not an archive
        ///// and will just be copied to _TempLocation for processing
        ///// </summary>
        ///// <param name="filePath">The full path of the file to extract</param>
        //private static void Extract(string filePath)
        //{
        //    // Confirm that arg 0 is a qualified file path
        //    var inputFile = new FileInfo(filePath);
        //    if (!inputFile.Exists)
        //    {
        //        throw new Exception("Input file does not exist");
        //    }
        //    // Open a new process to attempt a 7zip archive extract to a temp folder
        //    // The process must execute and wait for completion
        //    using var process = new Process();
        //    var startInfo = new ProcessStartInfo
        //    {
        //        WindowStyle = ProcessWindowStyle.Hidden,
        //        UseShellExecute = false,
        //        RedirectStandardOutput = true,
        //        FileName = "cmd.exe",
        //        Arguments = $"{@"/C"} \"\"{_7ZipExecutable}\" {@"e -bd -y -o"}\"{_TempLocation}\" \"{filePath}\"\""
        //    };
        //    Debug.Write($"Executing: {startInfo.FileName} {startInfo.Arguments}");
        //    process.StartInfo = startInfo;
        //    process.Start();
        //    process.WaitForExit();
        //    // Log the 7zip results
        //    Debug.Write("7zip result code: " + process.ExitCode);
        //    // Assume that 7zip errors mean that the file is not a zip archive and process as such
        //    if (process.ExitCode != 0)
        //    {
        //        Debug.Write("Not an archive? Processing as single file.");
        //        Directory.CreateDirectory(_TempLocation);
        //        File.Copy(filePath, Path.Combine(_TempLocation, inputFile.Name));
        //    }
        //}








        //  Deprecated main processing.
        //        /// <summary>
        //        /// Deprecated thingy.
        //        /// </summary>
        //        /// <returns></returns>
        //        private static List<DocM691_Invoice> MainProcessing()
        //        {
        //            var StatementRecords = new List<string>();
        //            var MemberRecords = new List<StringMap>();
        //            var BalFwdRecords = new Dictionary<string, List<BalFwdRecord>>();
        //            foreach (string filePath in Client.Files)
        //            {
        //                string filename = Path.GetFileName(filePath);
        //                string fileExtension = Path.GetExtension(filePath);
        //                Debug.Write($"Processing {filename}");
        //                //
        //                // Process the files into data structures
        //                //
        //                if (filename.StartsWith("Statement", StringComparison.InvariantCultureIgnoreCase))
        //                {
        //                    MCSB691.AddRange(ImportRows(filePath));
        //                }
        //                else if (filename.StartsWith("Balance", StringComparison.InvariantCultureIgnoreCase))
        //                {
        //                    const string memberIdHeader = "Member ID";
        //                    const string nameHeader = "Member Name";
        //                    const string contractIdHeader = "Contract ID";
        //                    const string accountIdHeader = "Account ID";
        //                    const string startDateHeader = "From Date";
        //                    const string endDateHeader = "To Date";
        //                    const string outstandingHeader = "Outstanding Amount";
        //                    const string dueDateHeader = "Due Date";
        //                    const string daysOverdueHeader = "Days Overdue";
        //                    DocM504A_BalFwdRecord DictionaryToRecord(StringMap dict) =>
        //                        new DocM504A_BalFwdRecord
        //                        {
        //                            // MemberID = dict[memberIdHeader].PadLeft(9, '0'),
        //                            MemberName = dict[nameHeader],
        //                            // ContractID = dict[contractIdHeader].PadLeft(9, '0'),
        //                            AccountID = dict[accountIdHeader].PadLeft(9, '0'),
        //                            StartDate = Date.Parse(dict[startDateHeader]),
        //                            EndDate = Date.Parse(dict[endDateHeader]),
        //                            OutstandingAmount = DecimalParse(dict[outstandingHeader]),
        //                            // DueDate = Date.Parse(dict[dueDateHeader]),
        //                           // DaysOverdue = IntParse(dict[daysOverdueHeader])
        //                        };
        //                    string[] headers = { memberIdHeader, nameHeader, contractIdHeader, accountIdHeader, startDateHeader, endDateHeader, outstandingHeader, dueDateHeader, daysOverdueHeader };
        //                    HeaderSource<List<StringMap>, List<string>> Records504 = new HeaderSource<List<StringMap>, List<string>>
        //                        ImportCSVWithHeader(
        //                        filePath
        //                        , primaryKey: accountIdHeader
        //                        , delimiter: ","
        //                        , useQuotes: true
        //                        , headers);

        //                    MCSB504A = x(MemberRecords); // TO DO: ???
        //                    /****************************************
        //                     *          Into the weeds              *
        //                     * **************************************/

        //                }
        //                else if (fileExtension.Equals(".csv", StringComparison.InvariantCultureIgnoreCase))
        //                {
        //                    const string memberIdHeader = "MemberID";

        //                    HeaderSource<List<StringMap>, List<string>> MemberRecords = ImportCSVWithHeader(
        //                        filePath
        //                        , primaryKey: memberIdHeader
        //                        , delimiter: "|"
        //                        , useQuotes: false);

        //                    var uniqueKeyedCSVData = x(MemberRecords); // TO DO: ???

        //                    foreach (var kvPair in uniqueKeyedCSVData)
        //                    {
        //                        MCSB690.AddRange(kvPair.Value);
        //                    }
        //                }
        //                else
        //                {
        //                    throw new Exception($"Unexpected file found: {filePath}");
        //                }
        //            }
        //            if (MCSB690.Count == 0)
        //            {
        //                throw new Exception("Missing file MCSB690");
        //            }
        //            if (implementation == "S")
        //            {
        //                if (MCSB504A.Count == 0)
        //                {
        //                    throw new Exception("Missing file MCSB504A");
        //                }
        //                if (MCSB691.Count == 0)
        //                {
        //                    throw new Exception("Missing file MCSB691");
        //                }
        //            }
        //            //
        //            // Process the fileData into Document objects
        //            //
        //            var documents = new List<DocM691_Invoice>();
        //            foreach (StringMap CAE in MCSB690)
        //            {
        //                // Create a new document and initialize its values
        //                 documents.Add(new DocM691_Invoice(CAE)); // deprecated constructor called.
        //            }

        //            // Process this as a member update file if the option is set
        /*        //            if (MCSB691.Count == 0 && implementation == "W")
                //            {

                //                // Set all documents to the nomail exclusion
                //                //foreach (Document doc in documents)
                //                //{
                //                //    doc.premiumWithhold = "S";
                //                //}

                //                // Perform query of all current member ID in our database
                //                using DataTable uluroWebAccounts = GetClientAccounts.Execute();
                //                // Filter the document list based on this query
                //                for (int iDoc = (documents.Count - 1); (iDoc >= 0); --iDoc)
                //                {
                //                    DocM691_Invoice doc = documents[iDoc];
                //                    // If the docs account/member ID pair exists in the system already then remove it
                //                    if (uluroWebAccounts.Select($@"MEMBERID = '{doc.memberID}' AND ACCOUNTID = '{doc.accountNumber}'").Length > 0)
                //                    {
                //                        documents.RemoveAt(iDoc);
                //                    }
                //                }
                //            }
                //            //*/

        //            // If the option is not set then ensure the transactional file exists
        /*//            else if (MCSB691.Count == 0)
        //            {
        //                throw new Exception("Missing file MCSB691");
        //            }*/


        //            // Process this as a normal Statements file
        /*else
//            {
//                if (code == "S")
//                {
*/
        //                    // Remove documents with the nomail exclusion code "S" 
        /*
                //                    for (int iDoc = (documents.Count - 1); (iDoc >= 0); --iDoc)
                //                    {
                //                        DocM691_Invoice doc = documents[iDoc];
                //                        if (doc.premiumWithhold == "S")
                //                        {
                //                            documents.RemoveAt(iDoc);
                //                        }
                //                    }*/
        //                }
        //                // Get detail objects, append good ones to statements
        //                //    and report null returns to bad statement records.
        /*var bad691Records = new List<string>();
//                foreach (string BIE in MCSB691)
//                {
//                    // dereference a document to mutate its details.
//                    DocM691_Invoice doc = documents.FirstOrDefault(s => s.accountNumber == TrimSubstring(BIE, 29, 9));
//                    // Create a new detail object and initialize its values
//                    if (doc == null)
//                    {
//                        bad691Records.Add(BIE);
//                    }
//                    else
//                    {
//                        Detail_M691 detail = Detail_M691.Detail691(BIE);
//                        const int accountIdLength = 9;
//                        string paddedAccountId = doc.accountNumber.PadLeft(accountIdLength, '0');
//                        if (MCSB504A.ContainsKey(paddedAccountId))
//                        {
//                            detail.balanceForward = MCSB504A[paddedAccountId].Sum(record => record.OutstandingAmount);
//                        } // To DO: room for an else branch.
//                        doc.details.Add(detail);
//                        // doc details mutated.
//                    }
//                }*/

        //                // add sum of balance forward details to statement as one detail, or report on bad balfwd data
        /*var bad504Records = new List<Record504A>();
//                foreach (KeyValuePair<string, List<Record504A>> kvPair in MCSB504A)
//                {
//                    List<Record504A> records = kvPair.Value;
//                    string accountId = kvPair.Value[0].AccountID;
//                    Document doc = documents.FirstOrDefault(s => s.accountNumber == accountId);
//                    if (doc == null)
//                    {
//                        bad504Records.AddRange(records);
//                    }
//                    else
//                    {
//                        foreach (Record504A record in records)
//                        {
//                            Detail detail = Detail.Detail504A(record);
//                            doc.details.Add(detail);
//                            // TODO: Sum the 504 details later
//                        }
//                    }
//                }*/

        //                // Email the bad 691 records
        /*#if !DEBUG
        //                if (bad691Records.Count > 0)
        //                {
        //                    var errorEmail = new MetroEmail();
        //                    string badOutFileName = $@"\\uluro-prod\SUBMIT\prt\Client Bad Statement Records {DateTime.Now:yyyyMMdd}.csv";
        //                    errorEmail.To.Add(@"ulurotech@metropresort.com");
        //                    errorEmail.Subject = @"Client Processing Bad Records";
        //                    errorEmail.Body = $"Hello,\r\nPlease upload the file located at {badOutFileName} to Client's SFTP and notify them of the upload.\r\nThank You,\r\nClient Statements PreProcessing";
        //                    using (var badOut = new StreamWriter(badOutFileName, false))
        //                    {
        //                        foreach (string s in bad691Records)
        //                        {
        //                            badOut.WriteLine(s);
        //                        }
        //                    }
        //                    errorEmail.Send();
        //                }
        //#endif    */


        //                /* Remove empty documents (documents with no details)
        /*                for (int iDoc = (documents.Count - 1); (iDoc >= 0); --iDoc)
        //                {
        //                    DocM691_Invoice doc = documents[iDoc];
        //                    if (doc.details.Count == 0)
        //                    {
        //                        documents.RemoveAt(iDoc);
        //                    }
        //                }*/

        //                // Perform any extra processing on the resulting documents
        /*                for (int iDoc = (documents.Count - 1); (iDoc >= 0); --iDoc)
        //                {
        //                    DocM691_Invoice doc = documents[iDoc];

        //                //  Set due dates per client rules.
        //                    string statementDate = doc.details.Max(m => m.fromDate).ToString("yyyy-MM-dd");
        //                    Date dueDate = Date.Parse(statementDate).AddMonths(1).Subtract(TimeSpan.FromDays(1));
        //                    doc.statementDate = statementDate;
        //                    doc.dueDate = dueDate.ToString("yyyy-MM-dd");

        //                //  Low income subsidy.
        //                    doc.balanceDue = doc.details.Sum(s => s.balanceDue + s.lateEnrollmentPenalty - s.lowIncomeSubsidy);

        //                //  Reset time.
        //                    DateTime now = DateTime.Now;

        //                //  Set document aging per client rules.
        //                    doc.agingFullPastDue = doc.details.Where(w => (now - w.toDate).TotalDays > 30).Sum(s => s.balanceDue + s.lateEnrollmentPenalty - s.lowIncomeSubsidy);

        //                }*/

        //                // Remove documents with a total due of $0
        /*                for (int iDoc = (documents.Count - 1); (iDoc >= 0); --iDoc)
        //                {
        //                    DocM691_Invoice doc = documents[iDoc];
        //                    if (doc.balanceDue == 0)
        //                    {
        //                        documents.RemoveAt(iDoc);
        //                    }
        //                    else
        //                    {
        //                        List<Detail_M691> details = doc.details;
        //                        for (int iDetail = (details.Count - 1); (iDetail >= 0); --iDetail)
        //                        {
        //                            Detail_M691 detail = details[iDetail];
        //                            if ((detail.balanceForward == 0) && (detail.balanceDue == 0) && (detail.lateEnrollmentPenalty == 0) && (detail.lowIncomeSubsidy == 0))
        //                            {
        //                                details.RemoveAt(iDetail);
        //                            }
        //                        }
        //                    }
        //                }*/
        //            }
        //            return documents;
        //        }

        /// <summary>
        /// Clean up file IO artifacts. (Called on Program.cs Finally)
        /// </summary>
        private static void CleanUp()
        {
            // Clean up any temp files
            if (Directory.Exists(ZipFiles._TempLocation))
            {
                Directory.Delete(ZipFiles._TempLocation, true);
            }
        }
    }



}
