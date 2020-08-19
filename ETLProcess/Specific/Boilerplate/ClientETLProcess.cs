using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

using ETLProcess.General;
using ETLProcess.General.Interfaces;
using ETLProcess.General.IO;
using ETLProcess.General.Containers;
using ETLProcess.General.Algorithms;
using ETLProcess.General.Profiles;

using String = System.String;
using AcctID = System.String;
using MemberID = System.String;
using ETLProcess.General.Containers.AbstractClasses;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace ETLProcess.Specific.Boilerplate
{
    /// <summary>
    /// A boilerplate example of a class to fulfill Client statement and welcome letter document types.
    /// </summary>
    internal sealed class ClientETLProcess : DataSet, IC_CSVFileIn<IO_FilesIn>
    {
        //Class members.
        private readonly IO_FilesIn Process_FilesIn;
        private FileDataRecords<Record_Statement, ClientETLProcess> statementRecords; // is a DataTable.
        private FileDataRecords<Record_Members, ClientETLProcess> memberRecords;
        private FileDataRecords<Record_BalFwd, ClientETLProcess> balFwdRecords;

        // Key Columns of each class (not including indexers).
        public Dictionary<Type, SampleColumnTypes> SampleColumns { get; } = null;
        // Public parameterless constructor, for inheritance.
        public ClientETLProcess() : base(IOFiles.PrepGuid.ToString()) { }
        public static string argIn = "";

        // Constructor for boilerplate implementation class files, required by interface.
        public ClientETLProcess(string arg)
            : base(IOFiles.PrepGuid.ToString())
        {
            Record_Statement.InitSample();
            Record_Members.InitSample();
            Record_BalFwd.InitSample();
            SampleColumns = new Dictionary<Type, SampleColumnTypes> {
                 { typeof(Record_Statement), new SampleColumnTypes((Record_Statement.Sample).columnTypes) }
                ,{ typeof(Record_Members), new SampleColumnTypes((Record_Members.Sample).columnTypes) }
                ,{ typeof(Record_BalFwd), (Record_BalFwd.Sample).columnTypes }
            };
            argIn = arg;

            // TO DO: MetroEmail.InitClient(); Does mandrill email the client when this goes off?
            IO_FilesIn.Init(new object[] { arg });
            this.Process_FilesIn = IO_FilesIn.GetDerivedInstance();

            Process_FilesIn.Check_Input(CheckFiles_Delegate);

            if (!Process_FilesIn.Check_Input(CheckFiles_Delegate)) {
                Log.WriteException("Bad file count in zipfile.");
            } // specified check for implementation Statement/WelcomeLetters
            Log.Write("Client ETL Profile Loaded.");
        }

        internal void ExportRecords()
        {
            XML.Export("out", this);
        }

        // Member interface for delegate to check files for requirements.
        public DelRet<bool, string[]> CheckFiles_Delegate { get; } =
            (string[] files) => { 
                if (files.Length != 3) {
                    throw new Exception("Wrong number of files, expected 3");
                }
                return true;
            };

        //public bool Check_Input(DelRet<bool, string[]> inputs)
        //{
        //    return Process_FilesIn.Check_Input(inputs);
        //}


        /// <summary>
        /// Return an enumeration of which document type it is.
        /// <br>Implementation Specific</br>
        /// </summary>
        /// <param name="filename">Filename of the file in question.</param>
        /// <returns></returns>
        public RecordType IdentifyRecordFile(string filename)
        {
            if (Path.GetFileNameWithoutExtension(filename).StartsWith("Statement", StringComparison.InvariantCultureIgnoreCase)) { 
                return RecordType.Statements; 
            }
            if (Path.GetFileNameWithoutExtension(filename).StartsWith("Member", StringComparison.InvariantCultureIgnoreCase)) { 
                return RecordType.Members; 
            }
            if (Path.GetFileNameWithoutExtension(filename).Contains("Balance")) { 
                return RecordType.BalancesForward; 
            }
            return RecordType.Error; // error code;
        }

        /// <summary>
        /// Public call to populate docs.
        /// </summary>
        public void PopulateRecords() 
        {
            PopulateRecords(Process_FilesIn.Files);
            Log.Write("Records Populated.");
        }

        /// <summary>
        /// Populate documents with information, in the proper order.
        /// </summary>
        /// <param name="files"></param>
        private void PopulateRecords(string[] files)
        {
            RecordType recordType;
            string filename
                , fileExtension;
            Queue<string> fileList = OrderFileList(files);

            foreach (string filePath in fileList)
            {
                filename = Path.GetFileName(filePath);
                fileExtension = Path.GetExtension(filePath);
                recordType = IdentifyRecordFile(filename);
                
                // put each document type into its headersource (struct of Stringmap and headers list)
                switch (recordType)
                {
                    case (RecordType.Statements):

                        List<string> StatementRecordData = CSV.ImportRows(filePath);

                        HeaderSource<List<StringMap>, List<string>> statementSrcData =
                            Record_Statement.Sample.ParseRows(StatementRecordData.ToArray());
                        statementRecords = new FileDataRecords<Record_Statement, ClientETLProcess>(
                            statementSrcData
                            , new ForeignKeyConstraintElements(this, typeof(Record_Statement).Name));
                        Log.Write("Statement Records files populated.");
                        break;

                    case (RecordType.Members):
                        // TO DO: once-over post-DataSet/DataTable type changes.
                        // statementTable already populated, per OrderFileList(files) method.

                        var membersByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , delimiter: "|"
                            , useQuotes: false);

                        memberRecords = new FileDataRecords<Record_Members, ClientETLProcess>(
                            membersByAcctID
                            , new ForeignKeyConstraintElements(
                                this
                                , new DataColumn[] { Tables[statementRecords.TableName].Columns["Group Billing Acct ID"] } // parent key columns.
                                , new String[] { "Group Billing Acct ID" } )
                            );
                        Log.Write("Member Records populated.");
                        break;

                    case (RecordType.BalancesForward):
                        // TO DO: once-over post-DataSet/DataTable type changes.
                        // balfwdfile has no internal headers.
                        List<string> headers = Record_BalFwd.Sample.headers;

                        // TO DO: 
                        var balFwdByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , ","
                            , useQuotes: true
                            , headers);

                        balFwdRecords = new FileDataRecords<Record_BalFwd, ClientETLProcess>(
                            balFwdByAcctID
                            , new ForeignKeyConstraintElements(
                                this
                                , new DataColumn[] { Tables[statementRecords.TableName].Columns["Group Billing Acct ID"] }
                                , new String[] { "Billing Account Number" } )
                            );
                        Log.Write("Balance Forward Records populated.");
                        break;

                    case (RecordType.Error):
                        throw new Exception($"Unexpected file found: {filePath}");
                }
            }
        }

        /// <summary>
        /// <Pre>PopulateDocs has already gotten all records and made them into tables.</Pre>
        /// <Post>A schema has been built around the relations of these tables.<br>
        /// Any relevant filters have been run, and reports have been made on bad data.</br></Post>
        /// </summary>
        /// <returns></returns>
        internal List<IOutputDoc> ProcessRecords()
        {
            List<IOutputDoc> outputDocs = new List<IOutputDoc>(); // Generate list of output documents with detail included.

            // PopulateDocs has already:
            // Got All Invoices, Bal Fwd Records, Member Records, and made them into tables.
            var newMembers = new List<Record_Members>();
            var invoices_MissingMembers = new List<Record_Statement>();
            var balances_MissingMembers = new List<Record_BalFwd>();
            ClientBusinessRules.Filter_MissingMembers(this, this.Tables[typeof(Record_Statement).ToString()], invoices_MissingMembers);
            ClientBusinessRules.Filter_MissingMembers(this, this.Tables[typeof(Record_BalFwd).ToString()], balances_MissingMembers);
            ClientBusinessRules.CheckOldClientAccounts(this, newMembers);
            // TO DO: report on missing members
            // TO DO: Output docs

            foreach (DataRow row in statementRecords.Rows)
            {

            }

            // programming progress exception
            throw new NotImplementedException("More polish to be implemented.");
        }

        /// <summary>
        /// Specific ETLProcess implementations need to order the entry of each file into the table, so the foreign key constraints
        ///     will be added in order.
        /// </summary>
        /// <param name="files">The array of filenames to be processed.</param>
        /// <returns>A <see cref="Queue{T}"/> which orders the files to be processed into tables.</returns>
        public Queue<string> OrderFileList(string[] files)
        {
            var dict = new Dictionary<string, int> ();
            foreach (string file in files)
            {
                RecordType docType = IdentifyRecordFile(file);
                switch (docType)
                {
                    case RecordType.Statements:
                        dict.Add(file, 0);
                        break;
                    case RecordType.Members:
                        dict.Add(file, 1);
                        break;
                    case RecordType.BalancesForward:
                        dict.Add(file, 2);
                        break;
                }
            }

            var ret = new Queue<String>();
            while (dict.Count > 0)
            {
                int minInt = dict.Values.Min();
                string min = dict.Keys.Where((x) => dict[x] == minInt).FirstOrDefault(); // allows multiple keys hashing to a bucket int value.
                ret.Enqueue(min);
                dict.Remove(min);
            }
            return ret;
        }
    }
}
