using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicETLProcess.General.Containers;
using BasicETLProcess.General.Interfaces;

// is this class needed? Does it even make sense in a 2d record?
namespace BasicETLProcess.Specific
{
    using BalanceForward = System.Decimal;
    internal class ClientMergedStatementRecord : BasicRecord, IRecord<ClientMergedStatementRecord>
    //where T1 : BasicDoc, iDocType_Takes_StringMap<DocM691_Invoice>, new()
    //where T2 : BasicDoc, iDocType_Takes_StringMap<DocM691_Invoice>, new()
    //where T3 : BasicDoc, iDocType_Takes_StringMap<DocM691_Invoice>, new()
    {

        private static readonly List<string> Headers = new string[] {
            "Group Billing Acct ID"
            , "invoiceNum"
            , "invoiceAmount"
            , "lateEnrollmentPenalty"
            , "lowIncomeSubsidy"
            , "fromDate"
            , "toDate"
            , "AccountID"
            , "StartDate"
            , "Balance"
            , "FullName"
            , "Address"
            , "Adjustments"
        }.ToList();

        /// <summary>
        /// Satisfies interface requirement for headers accessor to above readonly Headers member.
        /// </summary>
        public List<string> headers
        {
            get { return Headers; }
        }

        public ClientMergedStatementRecord() : base(
            headers: Headers
            , keyHeaders: new string[] { "AccountID", "StartDate" }.ToList()
            , null as Dictionary<string, string>
            , true) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="M691_Invoices"></param>
        /// <param name="M690_MemberFiles"></param>
        /// <param name="M504A_BalanceForwards"></param>
        public ClientMergedStatementRecord(
                KeyedRecords<StatementRecords> M691_Invoices
                , KeyedRecords<MemberRecords> M690_MemberFiles
                , KeyedRecords<BalFwdRecords> M504A_BalanceForwards
            ) : base(
                headers: Headers
                , keyHeaders: new string[] { "AccountID", "StartDate" }.ToList()
                , data: null as Dictionary<string, string>
                , true)
        {
            // hdrs check
            // instantiate CMSR with pre-filtered records:
            // instantiate CMSR by member file
            // populate statements
            // populate balances forward and any missing fields for blank statements

            throw new NotImplementedException();
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="stringMap"></param>
        /// <param name="headers"></param>
        ClientMergedStatementRecord(Dictionary<string, string> stringMap, List<string> headers)
            : base(
                  headers
                  , new string[] { "AccountID", "StartDate" }.ToList()
                  , stringMap
                  , true)
        {
            // hdrs check
            throw new NotImplementedException();

        }

        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface <see cref="IRecord{DocM690_MemberRecord}"/></br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="headers">The column headers</param>
        /// <returns></returns>
        /// <returns></returns>
        public ClientMergedStatementRecord Record(Dictionary<string, string> stringMap, List<string> headers)
        {
            return new ClientMergedStatementRecord(stringMap, headers);
        }
    }
}
