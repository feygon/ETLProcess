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
    internal class ClientMergedStatementRecord : BasicRecord, IRecord<ClientMergedStatementRecord>
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

        public ClientMergedStatementRecord() : base(null, new string[] { "AccountID", "StartDate" }.ToList(), true) { }


        public ClientMergedStatementRecord(
                KeyedRecords<StatementRecords> M691_Invoices
                , KeyedRecords<MemberRecords> M690_MemberFiles
                , KeyedRecords<BalFwdRecords> M504A_BalanceForwards
            ) : base(null, new string[] { "AccountID", "StartDate" }.ToList(), true)
        {

            // instantiate AMSR with pre-filtered records:
            // instantiate AMSR by member file
            // populate statements
            // populate balances forward and any missing fields for blank statements

            throw new NotImplementedException();
        }

        ClientMergedStatementRecord(Dictionary<string, string> stringMap, List<string> headers)
            : base(null, new string[] { "AccountID", "StartDate" }.ToList(), true)
        {

        }

        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface <see cref="IRecord{DocM690_MemberRecord}"/></br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="headers">The column headers</param>
        /// <returns></returns>
        /// <returns></returns>
        public ClientMergedStatementRecord GetRecord(Dictionary<string, string> stringMap, List<string> headers)
        {
            return new ClientMergedStatementRecord(stringMap, headers);
        }
    }
}
