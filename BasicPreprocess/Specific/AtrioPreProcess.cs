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
    /// A class to fulfill Atrio statement and welcome letter document types.
    /// </summary>
    public class AtrioPreProcess : IPreP_Specific<XMLPreProcess_Takes_Files>
    {
        private static string implementation;

        internal (KeyedDocs<DocM691_Invoice>
        , KeyedDocs<DocM690_MemberRecord>
        , KeyedDocs<DocM504A_BalFwdRecord>) docs;

        private XMLPreProcess_Takes_Files PreP;
        /// <summary>
        /// Constructor for boilerplate implementation class files, required by interface.
        /// </summary>
        /// <param name="PreP">The generic pre-processor</param>
        public AtrioPreProcess(XMLPreProcess_Takes_Files PreP, string arg)
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

        // Don't worry about it being grayed out. The promise of an interface necessitates this arrangement.
        /// <summary>
        /// Delegate to check files for requirements -- in this case, number of files.
        /// </summary>
        private DelRet<bool, string> checkFilesDelegate = (string[] files) => 
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
        public DelRet<bool, string> checkFiles_Delegate
        {
            get { return checkFilesDelegate; }
        }



        /// <summary>
        /// Return an enumeration of which document type it is.
        /// <br>Implementation Specific</br>
        /// </summary>
        /// <param name="filename">Filename of the file in question.</param>
        /// <returns></returns>
        public DocType IdentifyDoc(string filename)
        {
            if (filename.StartsWith("MCSB691", StringComparison.InvariantCultureIgnoreCase)) { return DocType.M691; }
            if (filename.StartsWith("MCSB504A", StringComparison.InvariantCultureIgnoreCase)) { return DocType.M504A; }
            if (filename.Contains("Member Billing")) { return DocType.M690; }
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
            KeyedDocs<DocM691_Invoice>
            , KeyedDocs<DocM690_MemberRecord>
            , KeyedDocs<DocM504A_BalFwdRecord>> PopulateDocs(string[] files)
        {
            // each will be a dictionary of documents indexed by their respective IDs.
            Dictionary<AcctID, DocM691_Invoice> MCSB691
                = new Dictionary<AcctID, DocM691_Invoice>();
            KeyedDocs<DocM691_Invoice> M691Records = null;
            KeyedDocs<DocM690_MemberRecord> M690Records = null;
            KeyedDocs<DocM504A_BalFwdRecord> M504Records = null;
            Dictionary<AcctID, DocM504A_BalFwdRecord> MCSB504A =
                new Dictionary<AcctID, DocM504A_BalFwdRecord>();
            var MCSB691_Primitive = new List<string>();
            var MSCB690_CSVData = new Dictionary<string, List<StringMap>>();

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
                    case (DocType.M691):
                        
                        MCSB691_Primitive = CSV.ImportRows(filename);
                        DocM691_Invoice interfaceDoc = new DocM691_Invoice();
                        HeaderSource<List<StringMap>, List<string>> src691 =
                            interfaceDoc.ParseRows(MCSB691_Primitive.ToArray());
                        M691Records = new KeyedDocs<DocM691_Invoice>(src691);
                        break;

                    case (DocType.M690):

                        var M690sByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , primaryKey: "Group Billing Acct ID"
                            , delimiter: "|"
                            , useQuotes: false);
                        M690Records = new KeyedDocs<DocM690_MemberRecord>(M690sByAcctID);
                        break;

                    case (DocType.M504A):
                        string[] headers = DocM504A_BalFwdRecord.headers;

                        var M504AsByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , primaryKey: "Account Id"
                            , ","
                            , useQuotes: true
                            , headers);

                        M504Records = new KeyedDocs<DocM504A_BalFwdRecord>(M504AsByAcctID);
                        break;

                    case (DocType.Error):
                        throw new Exception($"Unexpected file found: {filePath}");
                }
            }
            return new ValueTuple<
                KeyedDocs<DocM691_Invoice>
                , KeyedDocs<DocM690_MemberRecord>
                , KeyedDocs<DocM504A_BalFwdRecord>>(
                M691Records, M690Records, M504Records);
        }


        internal (
                KeyedDocs<AtrioMergedStatementRecord> merged
                , KeyedDocs<DocM691_Invoice> invoices_MissingMembers
                , KeyedDocs<DocM504A_BalFwdRecord>balances_MissingMembers)
            ProcessDocs(
                (KeyedDocs<DocM691_Invoice>
                , KeyedDocs<DocM690_MemberRecord>
                , KeyedDocs<DocM504A_BalFwdRecord>) atrioDocs)
        {
            KeyedDocs<AtrioMergedStatementRecord> merged;
            KeyedDocs<DocM691_Invoice> validInvoices;
            KeyedDocs<DocM504A_BalFwdRecord> validBalances;
            List<DocM691_Invoice> invoices_MissingMembers;
            List<DocM504A_BalFwdRecord> balances_MissingMembers;


            // instantiate AMSR with primary unique keyed members.
            merged = GetMergedStatementRecord(atrioDocs.Item2);

            // populate AMSRrecords with composite-keyed statements matching member records (account ID, date_if_any)
            // instantiate report records: statements with missing member records, a List of DocM691_Invoice documents
            (List<DocM691_Invoice> invoice_MissingMembers         // List of invoices of missing members, for report.
               , KeyedDocs<DocM691_Invoice> invoice_validMembers  // KeyedDocs of invoices of valid members
            ) invoices = GetInvoices(atrioDocs);

            //(KeyedDocs<DocM691_Invoice> invoices_MissingMembers , List<StringMap> invoice_valid_List ) invoices = GetInvoices(atrioDocs);

            // get missing members and valid members.
            invoices_MissingMembers = invoices.invoice_MissingMembers;
            validInvoices = invoices.invoice_validMembers;

            // TO DO: add valid invoices to merged.


            (List<DocM504A_BalFwdRecord> balance_MissingMembers         // List of balances of missing members, for report.
               , KeyedDocs<DocM504A_BalFwdRecord> balance_validMembers  // KeyedDocs of balances of valid members.
           ) balances = GetBalances(atrioDocs);


            // populate AMSRrecords with composite-indexed-keyed outstanding balances (account ID, date_if_any, index)
            // instantiate report records: outstanding balances with missing member records, a list of DocM504A_BalFwdRecord documents
            balances_MissingMembers = balances.balance_MissingMembers; //*
            validBalances = balances.balance_validMembers;

            // programming progress exception
            throw new Exception("Constructors missing parameters.");
        }

        private (
                List<DocM504A_BalFwdRecord> balance_MissingMembers, KeyedDocs<DocM504A_BalFwdRecord> balance_validMembers //tuple
            ) GetBalances(
                (KeyedDocs<DocM691_Invoice> statementsNotAppearingInThisFilm
                , KeyedDocs<DocM690_MemberRecord> members
                , KeyedDocs<DocM504A_BalFwdRecord> balances) atrioDocs) // tuple
        {
            List<StringMap> validBalFwdRecords = new List<StringMap>();          // TO DO: populate me with member-matched balance fwd records only!
            List<DocM504A_BalFwdRecord> balFwdRecords_Missing = new List<DocM504A_BalFwdRecord>();  // TO DO: populate me with the rest.
            List<string> balFwdHeaders = new List<string>();
            // TO DO: populate above 3 members with castoff invoices
            //
            //
            //


            // wrap headers and stringmaplists
            HeaderSource<List<StringMap>, List<string>> validBalanceHeaderSource =
                new HeaderSource<List<StringMap>, List<string>>(validBalFwdRecords, balFwdHeaders.ToArray());
            ///////// Remakes to leave original data intact. That's why the constructor takes a wrapper. /////////
            KeyedDocs<DocM504A_BalFwdRecord> balances_ValidMembers = new KeyedDocs<DocM504A_BalFwdRecord>(validBalanceHeaderSource);

            (List<DocM504A_BalFwdRecord> balance_MissingMembers, KeyedDocs<DocM504A_BalFwdRecord> balance_validMembers) ret = (
                balFwdRecords_Missing, balances_ValidMembers);

            return ret;
        }

        // Not finished. Add filter algorithm.
        private (List<DocM691_Invoice> balance_MissingMembers
               , KeyedDocs<DocM691_Invoice> balance_validMembers
           ) GetInvoices(
            (KeyedDocs<DocM691_Invoice> statements
                , KeyedDocs<DocM690_MemberRecord> members
                , KeyedDocs<DocM504A_BalFwdRecord> balances) atrioDocs)
        {
            // get merged arguments for KeyedDocs
            List<StringMap> invoices_ValidMembers = new List<StringMap>(); // TO DO: populate me!
            List<DocM691_Invoice> invoices_MissingMembers = new List<DocM691_Invoice>(); // TO DO: populate me!
            List<string> invoiceHeaders = new List<string>();
            // TO DO: use atrioDocs to populate above 3 members with valid/castoff invoices

            HeaderSource<List<StringMap>, List<string>> validInvoiceHeaderSource =
                new HeaderSource<List<StringMap>, List<string>>(invoices_ValidMembers, invoiceHeaders.ToArray());

            KeyedDocs<DocM691_Invoice> balance_validMembers = new KeyedDocs<DocM691_Invoice>(validInvoiceHeaderSource);

            (List<DocM691_Invoice> balance_MissingMembers
               , KeyedDocs<DocM691_Invoice> balance_validMembers
           ) ret = (
                invoices_MissingMembers
                , balance_validMembers);
            return ret;

        }

        // Not finished. Add filter algorithm.
        private KeyedDocs<AtrioMergedStatementRecord> GetMergedStatementRecord(
                KeyedDocs<DocM690_MemberRecord> atrioMembers // tuple
            )
        {
            // get merged arguments for KeyedDocs
            List<StringMap> mergedRecords = new List<StringMap>();  // populate me with member records data only!
            List<string> mergedHeaders = new List<string>();        // populate me with all records!
            // TO DO: use atrioDocs to populate above 2 members.
            HeaderSource<List<StringMap>, List<string>> mergedHeaderSource =
                new HeaderSource<List<StringMap>, List<string>>(mergedRecords, mergedHeaders.ToArray());

            return new KeyedDocs<AtrioMergedStatementRecord>(mergedHeaderSource);
        }
    }
}
