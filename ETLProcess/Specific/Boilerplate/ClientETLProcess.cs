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

using String = System.String;
using AcctID = System.String;
using MemberID = System.String;
using System.ComponentModel.DataAnnotations;

namespace ETLProcess.Specific.Boilerplate
{
    /// <summary>
    /// A class to fulfill Client statement and welcome letter document types.
    /// </summary>
    public class ClientETLProcess : DataSet, IETLP_Specific<FilesIn_XMLOut>
    {
        internal KeyedRecords<StatementRecords> statementRecords;
        internal KeyedRecords<MemberRecords> memberRecords;
        internal KeyedRecords<BalFwdRecords> balFwdRecords;

        private readonly static Dictionary<Type, List<string>> KeyColumns = new Dictionary<Type, List<String>>
        {
            {typeof(StatementRecords), new string[]{ "Group Billing Acct ID", "Invoice Number" }.ToList() }
            ,{typeof(MemberRecords), new string[]{ "Billing Account Number" }.ToList() }
            ,{typeof(BalFwdRecords), new string[]{ "Account ID" }.ToList() }
        };
        /// <summary>
        /// Key Columns of each class (not including indexers).
        /// </summary>
        public Dictionary<Type, List<string>> keyColumns { get {
                return KeyColumns;
            }
        }

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

        Dictionary<Type, List<string>> IETLP_Specific<FilesIn_XMLOut>.keyColumns => throw new NotImplementedException();

        DelRetArray<bool, string> IETLP_Specific<FilesIn_XMLOut>.CheckFiles_Delegate => throw new NotImplementedException();

        /// <summary>
        /// Return an enumeration of which document type it is.
        /// <br>Implementation Specific</br>
        /// </summary>
        /// <param name="filename">Filename of the file in question.</param>
        /// <returns></returns>
        public DocType IdentifyRecordFile(string filename)
        {
            if (filename.StartsWith("Statement", StringComparison.InvariantCultureIgnoreCase)) { return DocType.Statements; }
            if (filename.StartsWith("Member", StringComparison.InvariantCultureIgnoreCase)) { return DocType.Members; }
            if (filename.Contains("Balance")) { return DocType.BalancesForward; }
            return DocType.Error; // error code;
        }

        /// <summary>
        /// Public call to populate docs.
        /// </summary>
        public void PopulateRecords() 
        {
            PopulateRecords(PreP.files, out statementRecords, out memberRecords, out balFwdRecords);
        }

        /// <summary>
        /// Populate documents with information.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="statementRecords">Records of client member invoices.</param>
        /// <param name="memberRecords">Records of client members</param>
        /// <param name="balFwdRecords">Records of client member balances forward.</param>
        private void PopulateRecords(string[] files,
            out KeyedRecords<StatementRecords> statementRecords,
            out KeyedRecords<MemberRecords> memberRecords,
            out KeyedRecords<BalFwdRecords> balFwdRecords
            )
        {
            statementRecords = null;
            memberRecords = null;
            balFwdRecords = null;
            // each will be a dictionary of documents indexed by their respective IDs.

            DocType docType;
            string filename
                , fileExtension;
            Queue<string> fileList = OrderFileList(files);

            foreach (string filePath in fileList)
            {
                filename = Path.GetFileName(filePath);
                fileExtension = Path.GetExtension(filePath);
                docType = IdentifyRecordFile(filename);
                ForeignKeyConstraintElements FKConstraint;
                // put each document type into its headersource (struct of Stringmap and headers list)
                switch (docType)
                {
                    case (DocType.Statements):


                        List<string> StatementRecordData = CSV.ImportRows(filePath);
                        StatementRecords sample = new StatementRecords();

                        FKConstraint = new ForeignKeyConstraintElements(this);

                        HeaderSource<List<StringMap>, List<string>> statementSrcData =
                            sample.ParseRows(StatementRecordData.ToArray());
                        statementRecords = new KeyedRecords<StatementRecords>(
                            statementSrcData, PreP.guid, (IETLP_Specific<IETLP>)this, FKConstraint);
                        break;

                    case (DocType.Members):

                        var membersByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , delimiter: "|"
                            , useQuotes: false);

                        FKConstraint = new ForeignKeyConstraintElements(
                            this
                            , new DataColumn[] { Tables[statementRecords.table.TableName].Columns["Group Billing Acct ID"] }
                            , new String[] { "Billing Account Number" });

                        memberRecords = new KeyedRecords<MemberRecords>(
                            membersByAcctID, PreP.guid, (IETLP_Specific<IETLP>)this, FKConstraint);
                        break;

                    case (DocType.BalancesForward):
                        // balfwdfile has no internal headers.
                        List<string> headers = new BalFwdRecords().headers;

                        // TO DO: 
                        var balFwdByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , ","
                            , useQuotes: true
                            , headers);

                            FKConstraint = new ForeignKeyConstraintElements(
                                this
                                , new DataColumn[] { Tables[statementRecords.table.TableName].Columns["Group Billing Acct ID"] }
                                , new String[] { "Billing Account Number" });

                        balFwdRecords = new KeyedRecords<BalFwdRecords>(
                            balFwdByAcctID, PreP.guid, (IETLP_Specific<IETLP>)this, FKConstraint);
                        break;

                    case (DocType.Error):
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
            var newMembers = new List<MemberRecords>();
            var invoices_MissingMembers = new List<StatementRecords>();
            var balances_MissingMembers = new List<BalFwdRecords>();
            ClientBusinessRules.Filter_MissingMembers(this, this.Tables[typeof(StatementRecords).ToString()], invoices_MissingMembers);
            ClientBusinessRules.Filter_MissingMembers(this, this.Tables[typeof(BalFwdRecords).ToString()], balances_MissingMembers);
            ClientBusinessRules.CheckOldClientAccounts(this, newMembers);
            // TO DO: report on missing members
            // TO DO: Output docs

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
                DocType docType = IdentifyRecordFile(file);
                switch (docType)
                {
                    case DocType.Statements:
                        dict.Add(file, 0);
                        break;
                    case DocType.Members:
                        dict.Add(file, 1);
                        break;
                    case DocType.BalancesForward:
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
