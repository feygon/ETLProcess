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
using System.Data;

namespace BasicPreprocess.Specific.Boilerplate
{
    using StringMap = Dictionary<string, string>;
    /// <summary>
    /// A class to fulfill Atrio statement and welcome letter document types.
    /// </summary>
    public class SpecificPreProcess : iPreProcess_From_Zip
    {

        /// <summary>
        /// Constructor for boilerplate implementation class files, required by interface.
        /// </summary>
        /// <param name="files"></param>
        public SpecificPreProcess(string[] files)
        {
            Files = files;
        }

        /// <summary>
        /// Debug Log accessor/mutator, required by interface.
        /// </summary>
        public ILog Debug {
            get { return Program.Debug ?? throw new NotImplementedException("Log not yet instantiated."); }
            set { Program.Debug = (Log)value; }
        }

        private string[] files;

        /// <summary>
        /// Filenames from zipped files.
        /// <br>required by interface.</br>
        /// </summary>
        public string[] Files {
            get { return files; }
            set { files = value; }
        }

        /// <summary>
        /// Set the location of the Debug Log
        /// </summary>
        /// <param name="filename">Filename of the zipFile of atrio documents to be processed.</param>
        public void SetDebug(string filename)
        {
            Debug = new Log($@"{Program.AssemblyDirectory}\{DateTime.UtcNow.Ticks}{Path.GetFileNameWithoutExtension(filename)}_Debug.log");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="atrioDocs"></param>
        /// <returns></returns>
        internal (
                KeyedDocs<MergedStatementRecord> merged
                , KeyedDocs<InvoiceData> invoices_MissingMembers
                , KeyedDocs<BalFwdRecord>balances_MissingMembers)
            ProcessDocs(
                (KeyedDocs<InvoiceData>
                , KeyedDocs<MemberRecord>
                , KeyedDocs<BalFwdRecord>) atrioDocs)
        {
            KeyedDocs<MergedStatementRecord> merged;
            KeyedDocs<InvoiceData> validInvoices;
            KeyedDocs<BalFwdRecord> validBalances;
            List<InvoiceData> invoices_MissingMembers;
            List<BalFwdRecord> balances_MissingMembers;


            // instantiate AMSR with primary unique keyed members.
            merged = GetMergedStatementRecord(atrioDocs.Item2);
            
            // populate AMSRrecords with composite-keyed statements matching member records (account ID, date_if_any)
            // instantiate report records: statements with missing member records, a List of DocM691_Invoice documents
            (List<InvoiceData> invoice_MissingMembers         // List of invoices of missing members, for report.
               , KeyedDocs<InvoiceData> invoice_validMembers  // KeyedDocs of invoices of valid members
            ) invoices = GetInvoices(atrioDocs);

            //(KeyedDocs<DocM691_Invoice> invoices_MissingMembers , List<StringMap> invoice_valid_List ) invoices = GetInvoices(atrioDocs);

            // get missing members and valid members.
            invoices_MissingMembers = invoices.invoice_MissingMembers;
            validInvoices = invoices.invoice_validMembers;

            // TO DO: add valid invoices to merged.


            (List<BalFwdRecord> balance_MissingMembers         // List of balances of missing members, for report.
               , KeyedDocs<BalFwdRecord> balance_validMembers  // KeyedDocs of balances of valid members.
           ) balances = GetBalances(atrioDocs);


            // populate AMSRrecords with composite-indexed-keyed outstanding balances (account ID, date_if_any, index)
            // instantiate report records: outstanding balances with missing member records, a list of DocM504A_BalFwdRecord documents
            balances_MissingMembers = balances.balance_MissingMembers; //*
            validBalances = balances.balance_validMembers;

            // programming progress exception
            throw new Exception("Constructors missing parameters.");
        }

        private (
                List<BalFwdRecord> balance_MissingMembers, KeyedDocs<BalFwdRecord> balance_validMembers //tuple
            ) GetBalances(
                (KeyedDocs<InvoiceData> statementsNotAppearingInThisFilm
                , KeyedDocs<MemberRecord> members
                , KeyedDocs<BalFwdRecord> balances) atrioDocs) // tuple
        {
            List<StringMap> validBalFwdRecords = new List<StringMap>();          // TO DO: populate me with member-matched balance fwd records only!
            List<BalFwdRecord> balFwdRecords_Missing = new List<BalFwdRecord>();  // TO DO: populate me with the rest.
            List<string> balFwdHeaders = new List<string>();
            // TO DO: populate above 3 members with castoff invoices
            //
            //
            //


            // wrap headers and stringmaplists
            HeaderSource<List<StringMap>, List<string>> validBalanceHeaderSource =
                new HeaderSource<List<StringMap>, List<string>>(validBalFwdRecords, balFwdHeaders.ToArray());
            ///////// Remakes to leave original data intact. That's why the constructor takes a wrapper. /////////
            KeyedDocs<BalFwdRecord> balances_ValidMembers = new KeyedDocs<BalFwdRecord>(validBalanceHeaderSource);

            (List<BalFwdRecord> balance_MissingMembers, KeyedDocs<BalFwdRecord> balance_validMembers) ret = (
                balFwdRecords_Missing, balances_ValidMembers);

            return ret;
        }

        // Not finished. Add filter algorithm.
        private (List<InvoiceData> balance_MissingMembers
               , KeyedDocs<InvoiceData> balance_validMembers
           ) GetInvoices(
            (KeyedDocs<InvoiceData> statements
                , KeyedDocs<MemberRecord> members
                , KeyedDocs<BalFwdRecord> balances) atrioDocs)
        {
            // get merged arguments for KeyedDocs
            List<StringMap> invoices_ValidMembers = new List<StringMap>(); // TO DO: populate me!
            List<InvoiceData> invoices_MissingMembers = new List<InvoiceData>(); // TO DO: populate me!
            List<string> invoiceHeaders = new List<string>();
            // TO DO: use atrioDocs to populate above 3 members with valid/castoff invoices

            HeaderSource<List<StringMap>, List<string>> validInvoiceHeaderSource =
                new HeaderSource<List<StringMap>, List<string>>(invoices_ValidMembers, invoiceHeaders.ToArray());

            KeyedDocs<InvoiceData> balance_validMembers = new KeyedDocs<InvoiceData>(validInvoiceHeaderSource);

            (List<InvoiceData> balance_MissingMembers
               , KeyedDocs<InvoiceData> balance_validMembers
           ) ret = (
                invoices_MissingMembers
                , balance_validMembers);
            return ret;

        }

        // Not finished. Add filter algorithm.
        private KeyedDocs<MergedStatementRecord> GetMergedStatementRecord(
                KeyedDocs<MemberRecord> atrioMembers // tuple
            )
        {
            // get merged arguments for KeyedDocs
            List<StringMap> mergedRecords = new List<StringMap>();  // populate me with member records data only!
            List<string> mergedHeaders = new List<string>();        // populate me with all records!
            // TO DO: use atrioDocs to populate above 2 members.
            HeaderSource<List<StringMap>, List<string>> mergedHeaderSource =
                new HeaderSource<List<StringMap>, List<string>>(mergedRecords, mergedHeaders.ToArray());

            return new KeyedDocs<MergedStatementRecord>(mergedHeaderSource);
        }
    }
}
