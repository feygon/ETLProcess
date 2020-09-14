using System;
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

        public DataSet Output_NewMembers(DataSet allMembersDataSet)
        {
            throw new NotImplementedException();
        }

        // Deprecated. Use Datatables.
        public static int Filter_MissingMembers(
            DataSet data
            , DataTable checkTable
            , out DataTable missingRecords)
        {
            throw new NotImplementedException();

            // TO DO: populate datatable out members with missing invoices
            //
            //
            //
        }

        public static void PremiumWithold(){
            // TO DO: Whatever this does.
            //
        }

        public static int Query_NewMembers(
            DataSet statementTable
            , out DataTable newMembers)
        {
            newMembers = new DataTable();
            throw new NotImplementedException();
            // TO DO:
            // call a DataTable of old clients, and compare the new client member file records.
            // Any that aren't in the old clients file is a new client.
        }

        /// <summary>
        /// Add a graceperiod to a statement end date, to get the due date of that statement.
        /// </summary>
        /// <param name="statementEndDate">The end date of the statement.</param>
        /// <param name="gracePeriod">The number of days of grace period after a statement before payment is due.</param>
        /// <returns>Returns a due Date.</returns>
        internal static DateTime GetDueDate(DateTime statementEndDate, int gracePeriod)
        {
            return statementEndDate.AddDays(gracePeriod);
        }

        internal static readonly int StatementGracePeriod = 15;
    }
}
