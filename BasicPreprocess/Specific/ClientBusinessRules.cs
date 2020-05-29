using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPreprocess.Specific
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

        public static void PremiumWithold(){
            // TO DO:
            //
        }

        public static void CheckOldClientAccounts()
        {
            // TO DO:
            // call a DataTable of old clients, and compare the new client member file records.
            // Any that aren't in the old clients file, exclude from statements (and put out a report of them?).
        }
    }
}
