using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicPreprocess.General.Containers;
using BasicPreprocess.General.Interfaces;

namespace BasicPreprocess.Specific
{
    using BalanceForward = System.Decimal;
    internal class AtrioMergedStatementRecord : BasicDoc, IDoc<AtrioMergedStatementRecord>
        //where T1 : BasicDoc, iDocType_Takes_StringMap<DocM691_Invoice>, new()
        //where T2 : BasicDoc, iDocType_Takes_StringMap<DocM691_Invoice>, new()
        //where T3 : BasicDoc, iDocType_Takes_StringMap<DocM691_Invoice>, new()
    {

        /*
691         MemberID	Last Name	First Name	Middle Name	Address1	Address2	City	State	Zip	Billing Account Number	Premium Withhold
504         Invoice Period From Date	Invoice Period To Date	Invoice Number	Group Billing Acct ID	Individual Billing Acct ID	Invoice Amount	Low-Income Subsidy Amount	Late-Enrollment Penalty Amount
690         Member Id	 "Member Name"	 "Contract Id"	 "Account Id"	 "Billing Period From Date"	 "Billing Period Thru Date"	 "Outstanding Amount"	 "Billing Period Due Date"	" ""Number of Days Overdue""									"
             */


        public string BillingAcctNum, MemberID, PremiumWithold;   // 
        public decimal decMembers;
        public Date dateMembers;
        public Address address;
        public List<BalanceForward> outstandingBalances;

        public AtrioMergedStatementRecord() : base(null, "AccountID", "StartDate", true) { }


        public AtrioMergedStatementRecord(
                KeyedDocs<DocM691_Invoice> M691_Invoices
                , KeyedDocs<DocM690_MemberRecord> M690_MemberFiles
                , KeyedDocs<DocM504A_BalFwdRecord> M504A_BalanceForwards
            ) : base(null, "AccountID", "StartDate", true)
        {

            // instantiate AMSR with pre-filtered records:
            // instantiate AMSR by member file
            // populate statements
            // populate balances forward and any missing fields for blank statements

            throw new NotImplementedException();
        }

        AtrioMergedStatementRecord(Dictionary<string, string> stringMap, string[] headers)
            : base(null, "AccountID", "StartDate", true)
        {

        }

        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface <see cref="IDoc{DocM690_MemberRecord}"/></br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="headers">The column headers</param>
        /// <returns></returns>
        /// <returns></returns>
        public AtrioMergedStatementRecord GetT(Dictionary<string, string> stringMap, string[] headers)
        {
            return new AtrioMergedStatementRecord(stringMap, headers);
        }
    }
}
