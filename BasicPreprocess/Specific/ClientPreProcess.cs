using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BasicPreprocess.General;
using BasicPreprocess.General.Interfaces;
using BasicPreprocess.General.IO;
using BasicPreprocess.General.Containers;
using BasicPreprocess.General.Algorithms;

using String = System.String;
using AcctID = System.String;
using MemberID = System.String;

namespace BasicPreprocess.Specific.Boilerplate
{
    using StringMap = Dictionary<string, string>;


    /// <summary>
    /// A class to fulfill Client statement and welcome letter document types.
    /// </summary>
    public class ClientPreProcess : IPreP_Specific<XMLPreProcess_Takes_Files>
    {
        private static string implementation;

        internal (KeyedRecords<StatementRecords>
        , KeyedRecords<MemberRecords>
        , KeyedRecords<BalFwdRecords>) docs;

        private XMLPreProcess_Takes_Files PreP;
        /// <summary>
        /// Constructor for boilerplate implementation class files, required by interface.
        /// </summary>
        /// <param name="PreP">The generic pre-processor</param>
        /// <param name="arg"></param>
        public ClientPreProcess(XMLPreProcess_Takes_Files PreP, string arg)
        {
            // TO DO: MetroEmail.InitClient(); Does mandrill email the client when this goes off?
            this.PreP = PreP;
            GetImplementation(arg);
        }

        /// <summary>
        /// Input various values into implementation codes based on preprocessor definitions.
        /// <br>Done this way to encapsulate customer requirements in preprocessor definitions.</br>
        /// </summary>
        public void GetImplementation(string imp_Arg = null)
        {
            implementation = imp_Arg;
#if WelcomeLetter
            implementation = "W";
#endif
#if Statement
            implementation = "S";
#endif
        }

        // Don't worry about it being grayed out.
        // Intellisense doesn't understand returning a signature as using it.
        // The promise of an interface necessitates this arrangement.
        /// <summary>
        /// Delegate to check files for requirements -- in this case, number of files.
        /// </summary>
        private DelRet<bool, string> CheckFilesDelegate = (string[] files) => 
        {
            switch (implementation) // customer requirement implementation performed in preprocessor definitions.
            {
                case "W" when (files.Length != 1):
                    throw new Exception("Wrong number of files, expected 1");
                case "S" when (files.Length != 3):
                    throw new Exception("Wrong number of files, expected 3");
            }
            return true;
        };

        /// <summary>
        /// Member interface for delegate to check files for requirements.
        /// </summary>
        public DelRet<bool, string> CheckFiles_Delegate
        {
            get { return CheckFilesDelegate; }
        }

        /// <summary>
        /// Return an enumeration of which document type it is.
        /// <br>Implementation Specific</br>
        /// </summary>
        /// <param name="filename">Filename of the file in question.</param>
        /// <returns></returns>
        public DocType IdentifyDoc(string filename)
        {
            if (filename.StartsWith("Statement", StringComparison.InvariantCultureIgnoreCase)) { return DocType.Statements; }
            if (filename.StartsWith("Balance", StringComparison.InvariantCultureIgnoreCase)) { return DocType.Members; }
            if (filename.Contains("Member")) { return DocType.BalancesForward; }
            return DocType.Error; // error code;
        }

        /// <summary>
        /// Public call to populate docs.
        /// </summary>
        public void PopulateDocs() 
        {
            docs = PopulateDocs(PreP.files);
        }

        /// <summary>
        /// Populate documents with information.
        /// </summary>
        /// <param name="files"></param>
        private ValueTuple<
            KeyedRecords<StatementRecords>
            , KeyedRecords<MemberRecords>
            , KeyedRecords<BalFwdRecords>> PopulateDocs(string[] files)
        {
            // each will be a dictionary of documents indexed by their respective IDs.
            KeyedRecords<StatementRecords> statementRecords = null;
            KeyedRecords<MemberRecords> memberRecords = null;
            KeyedRecords<BalFwdRecords> balFwdRecords = null;
            var StatementRecordData = new List<string>(); // it's used.

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
                    case (DocType.Statements):
                        
                        StatementRecordData = CSV.ImportRows(filename);
                        StatementRecords interfaceDoc = new StatementRecords();
                        HeaderSource<List<StringMap>, List<string>> statementSrcData =
                            interfaceDoc.ParseRows(StatementRecordData.ToArray());
                        statementRecords = new KeyedRecords<StatementRecords>(statementSrcData);
                        break;

                    case (DocType.BalancesForward):

                        var membersByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , primaryKey: "Group Billing Acct ID"
                            , delimiter: "|"
                            , useQuotes: false);
                        memberRecords = new KeyedRecords<MemberRecords>(membersByAcctID);
                        break;

                    case (DocType.Members):
                        List<string> headers = BalFwdRecords.headers;

                        var balFwdByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , primaryKey: "Account Id"
                            , ","
                            , useQuotes: true
                            , headers);

                        balFwdRecords = new KeyedRecords<BalFwdRecords>(balFwdByAcctID);
                        break;

                    case (DocType.Error):
                        throw new Exception($"Unexpected file found: {filePath}");
                }
            }
            return new ValueTuple<
                KeyedRecords<StatementRecords>
                , KeyedRecords<MemberRecords>
                , KeyedRecords<BalFwdRecords>>(
                statementRecords, memberRecords, balFwdRecords);
        }


        internal (
                KeyedRecords<ClientMergedStatementRecord> merged
                , KeyedRecords<StatementRecords> invoices_MissingMembers
                , KeyedRecords<BalFwdRecords>balances_MissingMembers)
            ProcessDocs(
                (KeyedRecords<StatementRecords>
                , KeyedRecords<MemberRecords>
                , KeyedRecords<BalFwdRecords>) clientRecords)
        {
            KeyedRecords<ClientMergedStatementRecord> merged;
            KeyedRecords<StatementRecords> validInvoices;
            KeyedRecords<BalFwdRecords> validBalances;
            List<StatementRecords> invoices_MissingMembers;
            List<BalFwdRecords> balances_MissingMembers;


            // instantiate AMSR with primary unique keyed members.
            merged = GetMergedStatementRecord(clientRecords.Item2);

            // populate AMSRrecords with composite-keyed statements matching member records (account ID, date_if_any)
            // instantiate report records: statements with missing member records, a List of DocM691_Invoice documents
            (List<StatementRecords> invoice_MissingMembers         // List of invoices of missing members, for report.
               , KeyedRecords<StatementRecords> invoice_validMembers  // KeyedDocs of invoices of valid members
            ) invoices = GetInvoices(clientRecords);

            //(KeyedDocs<DocM691_Invoice> invoices_MissingMembers , List<StringMap> invoice_valid_List ) invoices = GetInvoices(clientDocs);

            // get missing members and valid members.
            invoices_MissingMembers = invoices.invoice_MissingMembers;
            validInvoices = invoices.invoice_validMembers;

            // TO DO: add valid invoices to merged.


            (List<BalFwdRecords> balance_MissingMembers         // List of balances of missing members, for report.
               , KeyedRecords<BalFwdRecords> balance_validMembers  // KeyedDocs of balances of valid members.
           ) balances = GetBalances(clientRecords);


            // populate AMSRrecords with composite-indexed-keyed outstanding balances (account ID, date_if_any, index)
            // instantiate report records: outstanding balances with missing member records, a list of DocM504A_BalFwdRecord documents
            balances_MissingMembers = balances.balance_MissingMembers; //*
            validBalances = balances.balance_validMembers;

            // programming progress exception
            throw new Exception("Constructors missing parameters.");
        }

        private (
                List<BalFwdRecords> balance_MissingMembers, KeyedRecords<BalFwdRecords> balance_validMembers //tuple
            ) GetBalances(
                (KeyedRecords<StatementRecords> statementsNotAppearingInThisFilm
                , KeyedRecords<MemberRecords> members
                , KeyedRecords<BalFwdRecords> balances) clientRecords) // tuple
        {
            List<StringMap> validBalFwdRecords = new List<StringMap>();          // TO DO: populate me with member-matched balance fwd records only!
            List<BalFwdRecords> balFwdRecords_Missing = new List<BalFwdRecords>();  // TO DO: populate me with the rest.
            List<string> balFwdHeaders = new List<string>();
            // TO DO: populate above 3 members with castoff invoices
            //
            //
            //


            // wrap headers and stringmaplists
            HeaderSource<List<StringMap>, List<string>> validBalanceHeaderSource =
                new HeaderSource<List<StringMap>, List<string>>(validBalFwdRecords, balFwdHeaders.ToArray());
            ///////// Remakes to leave original data intact. That's why the constructor takes a wrapper. /////////
            KeyedRecords<BalFwdRecords> balances_ValidMembers = new KeyedRecords<BalFwdRecords>(validBalanceHeaderSource);

            (List<BalFwdRecords> balance_MissingMembers, KeyedRecords<BalFwdRecords> balance_validMembers) ret = (
                balFwdRecords_Missing, balances_ValidMembers);

            return ret;
        }

        // Not finished. Add filter algorithm.
        private (List<StatementRecords> balance_MissingMembers
               , KeyedRecords<StatementRecords> balance_validMembers
           ) GetInvoices(
            (KeyedRecords<StatementRecords> statements
                , KeyedRecords<MemberRecords> members
                , KeyedRecords<BalFwdRecords> balances) clientRecords)
        {
            // get merged arguments for KeyedDocs
            List<StringMap> invoices_ValidMembers = new List<StringMap>(); // TO DO: populate me!
            List<StatementRecords> invoices_MissingMembers = new List<StatementRecords>(); // TO DO: populate me!
            List<string> invoiceHeaders = new List<string>();
            // TO DO: use clientRecords to populate above 3 members with valid/castoff invoices

            HeaderSource<List<StringMap>, List<string>> validInvoiceHeaderSource =
                new HeaderSource<List<StringMap>, List<string>>(invoices_ValidMembers, invoiceHeaders.ToArray());

            KeyedRecords<StatementRecords> balance_validMembers = new KeyedRecords<StatementRecords>(validInvoiceHeaderSource);

            (List<StatementRecords> balance_MissingMembers
               , KeyedRecords<StatementRecords> balance_validMembers
           ) ret = (
                invoices_MissingMembers
                , balance_validMembers);
            return ret;

        }

        // Not finished. Add filter algorithm.
        private KeyedRecords<ClientMergedStatementRecord> GetMergedStatementRecord(
                KeyedRecords<MemberRecords> clientRecords // tuple
            )
        {
            // get merged arguments for KeyedDocs
            List<StringMap> mergedRecords = new List<StringMap>();  // populate me with member records data only!
            List<string> mergedHeaders = new List<string>();        // populate me with all records!
            // TO DO: use clientRecords to populate above 2 members.
            HeaderSource<List<StringMap>, List<string>> mergedHeaderSource =
                new HeaderSource<List<StringMap>, List<string>>(mergedRecords, mergedHeaders.ToArray());

            return new KeyedRecords<ClientMergedStatementRecord>(mergedHeaderSource);
        }
    }
}
