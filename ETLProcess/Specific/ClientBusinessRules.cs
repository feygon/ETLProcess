﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETLProcess.General;
using ETLProcess.General.Containers;
using ETLProcess.General.Containers.Members;

namespace ETLProcess.Specific
{
    class ClientBusinessRules
    {

        //  // seriously running the query for every single record where this is the case?
        // goofy.
        //            if (MCSB691.Count == 0 && implementation == "W")
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

        // Deprecated. Use Datatables.
        public static void Filter_MissingMembers<TRecord>(
            DataSet data
            , DataTable checkTable
            , List<TRecord> missingRecords) where TRecord: BasicRecord<TRecord>, new()
        {
            missingRecords ??= new List<TRecord>();

            // TO DO: populate ret members with missing invoices
            //
            //
            //
        }

        public static void PremiumWithold(){
            // TO DO: Whatever this does.
            //
        }

        public static void CheckOldClientAccounts(
            DataTable statementTable
            , DataTable memberTable
            , List<Record_Members> newMembers)
        {
            newMembers ??= new List<Record_Members>();
            // TO DO:
            // call a DataTable of old clients, and compare the new client member file records.
            // Any that aren't in the old clients file, exclude that member from statements (by ID) (and put out a report of them?).
        }

        internal static void CheckOldClientAccounts(DataSet data, List<Record_Members> newMembers)
        {
            newMembers ??= new List<Record_Members>();

            //TO DO: Make a sql call and check against it.
            //
            //
            //
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a graceperiod to a statement end date, to get the due date of that statement.
        /// </summary>
        /// <param name="statementEndDate">The end date of the statement.</param>
        /// <param name="gracePeriod">The number of days of grace period after a statement before payment is due.</param>
        /// <returns>Returns a due Date.</returns>
        internal static Date GetDueDate(Date statementEndDate, int gracePeriod)
        {
            return statementEndDate.AddDays(gracePeriod);
        }
        internal static readonly int StatementGracePeriod = 15;
    }
}