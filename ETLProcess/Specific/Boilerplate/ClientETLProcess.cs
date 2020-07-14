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
using SampleColumnTypes = System.Collections.Generic.Dictionary<string, System.Type>;
using ETLProcess.General.Containers.AbstractClasses;
using System.ComponentModel.DataAnnotations;

namespace ETLProcess.Specific.Boilerplate
{
    /// <summary>
    /// A class to fulfill Client statement and welcome letter document types.
    /// </summary>
    public class ClientETLProcess : DataSet, IETLP_Specific_FilesIn<FilesIn_XMLOut>, IETLP_Specific<SQLIn_XMLOut>, I_CSVIn
    {
        static readonly List<string> statementHeaders = new string[] { "Group Billing Acct ID", "Invoice Number" }.ToList();
        static readonly List<string> memberHeaders = new string[] { "Billing Account Number" }.ToList();
        static readonly List<string> balFwdHeaders = new string[] { "Account ID" }.ToList();

        /// <summary>
        /// Names of Key Columns in each Type
        /// </summary>
        public Dictionary<Type, List<string>> CSVColumnNames { get; } = new Dictionary<Type, List<string>>()
        {
            { typeof(Record_Statement), statementHeaders },
            { typeof(Record_Members), memberHeaders },
            { typeof(Record_BalFwd), balFwdHeaders }
        };

        internal DataRecords<Record_Statement> statementRecords; // is a DataTable.
        internal DataRecords<Record_Members> memberRecords;
        internal DataRecords<Record_BalFwd> balFwdRecords;

        /// <summary>
        /// Key Columns of each class (not including indexers).
        /// </summary>
        public Dictionary<Type, SampleColumnTypes> SampleColumns { get; } =
            new Dictionary<Type, SampleColumnTypes> {
                { typeof(Record_Statement), Record_Statement.Sample.columnTypes }
                ,{ typeof(Record_Members), Record_Members.Sample.columnTypes}
                ,{ typeof(Record_BalFwd), Record_BalFwd.Sample.columnTypes}
        };

        private readonly FilesIn_XMLOut PreP;
        /// <summary>
        /// Constructor for boilerplate implementation class files, required by interface.
        /// </summary>
        /// <param name="PreP">The generic pre-processor</param>
        public ClientETLProcess(FilesIn_XMLOut PreP) 
            : base(FilesIn_XMLOut.PrepGuid.ToString())
        {
            // TO DO: MetroEmail.InitClient(); Does mandrill email the client when this goes off?
            this.PreP = PreP;
        }

        internal void ExportRecords()
        {
            throw new NotImplementedException();
        }

        // Don't worry about it being grayed out.
        // Intellisense doesn't understand returning a signature as using it.
        // The promise of an interface necessitates this arrangement.
        /// <summary>
        /// Delegate to check files for requirements -- in this case, number of files.
        /// </summary>
        private readonly DelRetArray<bool, string> CheckFilesDelegate = (string[] files) => 
        {
            if (files.Length != 3)
            {
                throw new Exception("Wrong number of files, expected 3");
            }
            return true;
        };

        /// <summary>
        /// Member interface for delegate to check files for requirements.
        /// </summary>
        public DelRetArray<bool, string> CheckFiles_Delegate
        {
            get { return CheckFilesDelegate; }
        }

        DelRetArray<bool, string> IETLP_Specific_FilesIn<FilesIn_XMLOut>.CheckFiles_Delegate => throw new NotImplementedException();

        /// <summary>
        /// Return an enumeration of which document type it is.
        /// <br>Implementation Specific</br>
        /// </summary>
        /// <param name="filename">Filename of the file in question.</param>
        /// <returns></returns>
        public RecordType IdentifyRecordFile(string filename)
        {
            if (filename.StartsWith("Statement", StringComparison.InvariantCultureIgnoreCase)) { return RecordType.Statements; }
            if (filename.StartsWith("Member", StringComparison.InvariantCultureIgnoreCase)) { return RecordType.Members; }
            if (filename.Contains("Balance")) { return RecordType.BalancesForward; }
            return RecordType.Error; // error code;
        }

        /// <summary>
        /// Public call to populate docs.
        /// </summary>
        public void PopulateRecords() 
        {
            PopulateRecords(PreP.files);
        }

        /// <summary>
        /// Populate documents with information.
        /// </summary>
        /// <param name="files"></param>
        private void PopulateRecords(string[] files)
        {
            statementRecords = null;
            memberRecords = null;
            balFwdRecords = null;
            // each will be a dictionary of documents indexed by their respective IDs.

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
                        Record_Statement sample = new Record_Statement();

                        HeaderSource<List<StringMap>, List<string>> statementSrcData =
                            sample.ParseRows(StatementRecordData.ToArray());
                        statementRecords = new DataRecords<Record_Statement>(
                            statementSrcData
                            , this
                            , new ForeignKeyConstraintElements(this));
                        break;

                    case (RecordType.Members):
                        // TO DO: once-over post-DataSet/DataTable type changes.
                        // statementTable already populated, per OrderFileList(files) method.

                        var membersByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , delimiter: "|"
                            , useQuotes: false);

                        memberRecords = new DataRecords<Record_Members>(
                            membersByAcctID
                            , this
                            , new ForeignKeyConstraintElements(
                                this
                                , new DataColumn[] { Tables[statementRecords.TableName].Columns["Group Billing Acct ID"] } // parent key columns.
                                , new String[] { "Group Billing Acct ID" } )
                            );
                        break;

                    case (RecordType.BalancesForward):
                        // TO DO: once-over post-DataSet/DataTable type changes.
                        // balfwdfile has no internal headers.
                        List<string> headers = new Record_BalFwd().headers;

                        // TO DO: 
                        var balFwdByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , ","
                            , useQuotes: true
                            , headers);

                        balFwdRecords = new DataRecords<Record_BalFwd>(
                            balFwdByAcctID
                            , this
                            , new ForeignKeyConstraintElements(
                                this
                                , new DataColumn[] { Tables[statementRecords.TableName].Columns["Group Billing Acct ID"] }
                                , new String[] { "Billing Account Number" } )
                            );
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
            for (int i=0; i < dict.Count; i++)
            {
                string max = dict.Max().Key;
                ret.Enqueue(max);
                dict.Remove(max);
            }
            return ret;
        }
    }
}
