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
    Statement,
    /// <summary>
    /// Client's's Member File
    /// </summary>
    Members,
    /// <summary>
    /// Client's Balance Forward file
    /// </summary>
    BalanceForward,
    /// <summary>
    /// Catch-all error enumeration.
    /// </summary>
    Error = -42
}

///// <summary>
///// Enumeration of document key types
///// </summary>
//public enum DocKeyType
//{
//    primaryUnique,
//    primaryRedundant,
//    compositeUnique,
//    compositeRedundant,
//    Error = -42
//}

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
        private static string implementation;
        private static SpecificPreProcess atrio;

        static private (KeyedDocs<InvoiceData>
            , KeyedDocs<MemberRecord>
            , KeyedDocs<BalFwdRecord>) docs;


        private static void Main(string[] args)
        {
            try
            {
                // Get customer rules defined in preprocessor definitions.

                GetPreprocessorDefinitions();

                SetTempDir(args[0]);
                string[] files = ZipFiles.GetFiles(args[0]);

                atrio = new SpecificPreProcess(files);
                atrio.SetDebug(args[0]);
                
                atrio = new SpecificPreProcess(files);

                // get files into temp directory

                CheckFiles(atrio.Files); // len check for implementation Statement/WelcomeLetters

                // input data to 691, 504A, 690
                docs = PopulateDocs(atrio.Files);

                // TO DO: process them separately by filtering business rules
                // TO DO: process them together by key-based business rules
                atrio.ProcessDocs(docs);
                throw new Exception("Finish implementing ProcessDocs.");
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


        /// <summary>
        /// Check number of files.
        /// </summary>
        /// <param name="files">string array of filenames.</param>
        private static void CheckFiles(string[] files)
        {
            switch (implementation) // customer requirement implementation performed in preprocessor definitions.
            {
                case "W" when (files.Length != 1):
                    throw new Exception("Wrong number of files, expected 1");
                case "S" when (files.Length != 3):
                    throw new Exception("Wrong number of files, expected 3");
            }
        }

        /// <summary>
        /// Implements ZipFiles._TempLocation with path for zipfiles to go.
        /// <param>The filename to set the temp directory from.</param>
        /// </summary>
        private static void SetTempDir(string filename)
        {
            ZipFiles._TempLocation = $@"{Path.GetTempPath()}MetPrep_{DateTime.UtcNow.Ticks}{Path.GetFileNameWithoutExtension(filename)}";
            while (Directory.Exists(ZipFiles._TempLocation))
            {
                ZipFiles._TempLocation += @"_2";
            }
        }



        /// <summary>
        /// Input various values into implementation codes based on preprocessor definitions.
        /// <br>Done this way to encapsulate customer requirements in preprocessor definitions.</br>
        /// </summary>
        private static void GetPreprocessorDefinitions()
        {
#if WelcomeLetter
            implementation = "W";
#endif
#if Statement
            implementation = "S";
#endif
        }


        /// <summary>
        /// Populate 
        /// </summary>
        /// <param name="files"></param>
        private static ValueTuple<
            KeyedDocs<InvoiceData>
            , KeyedDocs<MemberRecord>
            , KeyedDocs<BalFwdRecord>> PopulateDocs(string[] files)
        {
            // each will be a dictionary of documents indexed by their respective IDs.
            KeyedDocs<InvoiceData> InvoiceData = null;
            KeyedDocs<MemberRecord> MemberData = null;
            KeyedDocs<BalFwdRecord> BalFwdData = null;
            var MCSB691_Primitive = new List<string>();

            DocType docType;
            string filename
                , fileExtension;
            foreach (string filePath in files)
            {
                filename = Path.GetFileName(filePath);
                fileExtension = Path.GetExtension(filePath);
                docType = IdentifyDoc(filename);

                // put each document type into its headersource (struct of Stringmap and headers list)
                switch (docType)
                {
                    case (DocType.Statement):

                        MCSB691_Primitive = ImportRows(filename);
                        HeaderSource<List<StringMap>, List<string>> src691 =
                            Specific.InvoiceData.ParseM691(MCSB691_Primitive.ToArray());
                        InvoiceData = new KeyedDocs<InvoiceData>(src691);
                        break;

                    case (DocType.BalanceForward):

                        var M690sByAcctID = ImportCSVWithHeader(
                            filePath
//                            , primaryKey: "Group Billing Acct ID"
                            , delimiter: "|"
                            , useQuotes: false);
                        MemberData = new KeyedDocs<MemberRecord>(M690sByAcctID);
                        break;

                    case (DocType.Members):
                        string[] headers = BalFwdRecord.headers;

                        var M504AsByAcctID = ImportCSVWithHeader(
                            filePath
//                            , primaryKey: "Account Id"
                            , ","
                            , useQuotes: true
                            , headers);

                        BalFwdData = new KeyedDocs<BalFwdRecord>(M504AsByAcctID);
                        break;

                    case (DocType.Error):
                        throw new Exception($"Unexpected file found: {filePath}");
                }
            }
            return new ValueTuple<
                KeyedDocs<InvoiceData> 
                , KeyedDocs<MemberRecord> 
                , KeyedDocs<BalFwdRecord>>(
                InvoiceData, MemberData, BalFwdData);
        }

        /// <summary>
        /// return an enumeration of which file it is.
        /// <br>Implementation Specific.</br>
        /// </summary>
        /// <param name="filename">Filename of the file in question.</param>
        /// <returns></returns>
        private static DocType IdentifyDoc(string filename)
        {
            if (filename.StartsWith("MCSB691", StringComparison.InvariantCultureIgnoreCase)) { return DocType.Statement; }
            if (filename.StartsWith("MCSB504A", StringComparison.InvariantCultureIgnoreCase)) { return DocType.Members; }
            if (filename.Contains("Member Billing")) { return DocType.BalanceForward; }
            return DocType.Error; // error code;
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

        /// <summary>
        /// Imports a single file by row. Each string object in the list is 1 row in the file.
        /// </summary>
        /// <param name="fileName">The full path of the file to import</param>
        /// <returns>A List of 1 string per row</returns>
        private static List<string> ImportRows(string fileName)
        {
            var result = new List<string>();
            using (Stream readFile = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (TextReader textReader = new StreamReader(readFile))
            {
                // Row parsing
                string input = "";
                while ((input = textReader.ReadLine()) != null)
                {
                    result.Add(input);
                }
            }
            return result;
        }


        /// <summary>
        /// Imports a single file as a delimited file with a header. Row 1 is always parsed as a header, and is used to construct resulting dictionaries
        /// by row. Each dict is row 1 as the keys and each following row of the file as the values.
        /// </summary>
        /// <param name="fileName">The full path of the file to import</param>
        /// <param name="delimiter">What is the delimiting character? i.e. comma, pipe, tab, etc.</param>
        /// <param name="useQuotes">Are there quotes around values?</param>
        /// <param name="headers">A preloaded set of headers -- optional.</param>
        /// <returns>A List of Dictionary per row where KEY=Row1</returns>
        private static HeaderSource<List<StringMap>, List<string>> ImportCSVWithHeader(
            string fileName
            , string delimiter
            , bool useQuotes
            , IList<string> headers = null)
        {
            using (Stream readFile = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // CSV Parsing
                var csvRead = new TextFieldParser(readFile)
                {
                    CommentTokens = new[] {"#"},
                    Delimiters = new[] {delimiter},
                    HasFieldsEnclosedInQuotes = useQuotes,
                    TextFieldType = FieldType.Delimited,
                    TrimWhiteSpace = true
                };

                // if header is null, replace header with csvRead.ReadFields(), or with empty string if that's null.
                
                if (headers == null) { headers = csvRead.ReadFields(); }
                if (headers == null) { headers = new string[] { }; }

                List<StringMap> records = new List<StringMap>();
                while (!csvRead.EndOfData)
                {
                    string[] rowData = csvRead.ReadFields() ?? new string[] { };
                    var newRow = new StringMap();
                    for (int n = 0; n < rowData.Length; ++n) // len = number of fields.
                    {
                        newRow.Add(headers[n], rowData[n]);
                    }
                    records.Add(newRow);
                }

                List<string> headerList = new List<string>(headers);
                HeaderSource<List<StringMap>, List<string>> ret = 
                    new HeaderSource<List<StringMap>, List<string>> (records, headerList.ToArray());
                

                return ret;

            }
        }

        /// <summary>
        /// Imports a single file as a delimited file. The list is a collection of the delimited rows.
        /// </summary>
        /// <param name="fileName">The ful path of the file to import</param>
        /// <param name="delimiter"></param>
        /// <param name="useQuotes"></param>
        /// <returns>A List of string[] per row</returns>
        private static List<string[]> ImportCSV(string fileName, string delimiter = ",", bool useQuotes = false)
        {
            var result = new List<string[]>();
            using (Stream readFile = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // CSV Parsing
                var csvRead = new TextFieldParser(readFile)
                {
                    CommentTokens = new[] {"#"},
                    Delimiters = new[] {delimiter},
                    HasFieldsEnclosedInQuotes = useQuotes,
                    TextFieldType = FieldType.Delimited,
                    TrimWhiteSpace = true
                };
                // Load result
                while (!csvRead.EndOfData)
                {
                    result.Add(csvRead.ReadFields());
                }
            }
            return result;
        }

//  Deprecated main processing.
//        /// <summary>
//        /// Deprecated thingy.
//        /// </summary>
//        /// <returns></returns>
//        private static List<DocM691_Invoice> MainProcessing()
//        {
//            var MCSB691 = new List<string>();
//            var MCSB690 = new List<StringMap>();
//            var MCSB504A = new Dictionary<string, List<DocM504A_BalFwdRecord>>();
//            foreach (string filePath in atrio.Files)
//            {
//                string filename = Path.GetFileName(filePath);
//                string fileExtension = Path.GetExtension(filePath);
//                Debug.Write($"Processing {filename}");
//                //
//                // Process the files into data structures
//                //
//                if (filename.StartsWith("MCSB691", StringComparison.InvariantCultureIgnoreCase))
//                {
//                    MCSB691.AddRange(ImportRows(filePath));
//                }
//                else if (filename.StartsWith("MCSB504A", StringComparison.InvariantCultureIgnoreCase))
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

//            //
//            // Process this as a member update file if the option is set
//            //
//            if (MCSB691.Count == 0 && implementation == "W")
//            {
				
//                // Set all documents to the nomail exclusion
//                //foreach (Document doc in documents)
//                //{
//                //    doc.premiumWithhold = "S";
//                //}
				
//                // Perform query of all current member ID in our database
//                using DataTable uluroWebAccounts = GetATRIOAccounts.Execute();
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
//            //
//            // If the option is not set then ensure the transactional file exists
//            //
//            else if (MCSB691.Count == 0)
//            {
//                throw new Exception("Missing file MCSB691");
//            }
//            //
//            // Process this as a normal Statements file
//            //
//            else
//            {
//                if (code == "S")
//                {
//                    // Remove documents with the nomail exclusion code "S"
//                    for (int iDoc = (documents.Count - 1); (iDoc >= 0); --iDoc)
//                    {
//                        DocM691_Invoice doc = documents[iDoc];
//                        if (doc.premiumWithhold == "S")
//                        {
//                            documents.RemoveAt(iDoc);
//                        }
//                    }
//                }
//                // Append detail objects
//                var bad691Records = new List<string>();
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
//                }
//                /*var bad504Records = new List<Record504A>();
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
//#if !DEBUG
//                // Email the bad 691 records
//                if (bad691Records.Count > 0)
//                {
//                    var errorEmail = new MetroEmail();
//                    string badOutFileName = $@"\\uluro-prod\SUBMIT\prt\ATRIO Bad 691 Records {DateTime.Now:yyyyMMdd}.csv";
//                    errorEmail.To.Add(@"ulurotech@metropresort.com");
//                    errorEmail.Subject = @"ATRIO Processing Bad Records";
//                    errorEmail.Body = $"Hello,\r\nPlease upload the file located at {badOutFileName} to ATRIO's SFTP and notify them of the upload.\r\nThank You,\r\nATRIO Statements PreProcessing";
//                    using (var badOut = new StreamWriter(badOutFileName, false))
//                    {
//                        foreach (string s in bad691Records)
//                        {
//                            badOut.WriteLine(s);
//                        }
//                    }
//                    errorEmail.Send();
//                }
//#endif
//                // Remove empty documents
//                for (int iDoc = (documents.Count - 1); (iDoc >= 0); --iDoc)
//                {
//                    DocM691_Invoice doc = documents[iDoc];
//                    if (doc.details.Count == 0)
//                    {
//                        documents.RemoveAt(iDoc);
//                    }
//                }
//                // Perform any extra processing on the resulting documents
//                for (int iDoc = (documents.Count - 1); (iDoc >= 0); --iDoc)
//                {
//                    DocM691_Invoice doc = documents[iDoc];
//                    string statementDate = doc.details.Max(m => m.fromDate).ToString("yyyy-MM-dd");
//                    Date dueDate = Date.Parse(statementDate).AddMonths(1).Subtract(TimeSpan.FromDays(1));
//                    doc.statementDate = statementDate;
//                    doc.dueDate = dueDate.ToString("yyyy-MM-dd");
//                    doc.balanceDue = doc.details.Sum(s => s.balanceDue + s.lateEnrollmentPenalty - s.lowIncomeSubsidy);
//                    DateTime now = DateTime.Now;
//                    doc.agingFullPastDue = doc.details.Where(w => (now - w.toDate).TotalDays > 30).Sum(s => s.balanceDue + s.lateEnrollmentPenalty - s.lowIncomeSubsidy);
//                }
//                // Remove 0 sum documents and details
//                for (int iDoc = (documents.Count - 1); (iDoc >= 0); --iDoc)
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
//                }
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
