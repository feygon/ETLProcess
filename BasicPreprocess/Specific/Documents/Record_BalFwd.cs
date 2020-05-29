using System;
using System.Diagnostics;
using System.Collections.Generic;
using BasicPreprocess.General.Interfaces;
using BasicPreprocess.General.Containers;
using System.Linq;

namespace BasicPreprocess.Specific
{
    using StringMap = Dictionary<string, string>;

    /// <summary>
    /// Container for a primary redundant keyed data set reflecting Client's balance forward records.
    /// </summary>
    internal sealed class BalFwdRecords : BasicRecord, IRecord<BalFwdRecords>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public static readonly List<string> headers = new string[]{
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
            "Member Id"
            ,"Member Name"
            ,"Contract Id"
            ,"Account Id"
            ,"Billing Period From Date"
            ,"Billing Period Thru Date"
            ,"Outstanding Amount"
            ,"Billing Period Due Date"
            ,"Number of Days Overdue" 
        }.ToList();

        //internal string MemberID { get; set; }
        internal string MemberName { get; set; }
        //internal string ContractID { get; set; }
        internal string AccountID { get; set; }
        internal Date StartDate { get; set; }
        internal Date EndDate { get; set; }
        internal decimal OutstandingAmount { get; set; }
        //internal Date DueDate { get; set; }
        //internal int DaysOverdue { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BalFwdRecords() : base(headers,
            new string[] { "Account ID" }.ToList(),
            keyIsUniqueIdentifier: false) { }

        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface <see cref="IRecord{DocM504A_BalFwdRecord}"/></br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="sirNotAppearingInThisFilm">A member which is unused in this implementation.</param>
        /// <returns></returns>
        public BalFwdRecords GetRecord(StringMap stringMap, List<string> sirNotAppearingInThisFilm = null)
        {
            return new BalFwdRecords(stringMap);
        }

        /// <summary>
        /// Constructor, takes Stringmap -- not yet implemented.
        /// </summary>
        /// <param name="init"></param>
        public BalFwdRecords(StringMap init)
            : base(headers, 
                  new string[] { "Account ID" }.ToList(),
                  keyIsUniqueIdentifier: false)
        {
            this.AccountID = init["Account Id"] ?? "Bad header on AccountID in DocM504A";
            this.MemberName = init["Member Name"] ?? "Bad header on MemberName in DocM504A";
            this.OutstandingAmount = BasicRecord.GetDecimalColumn(init, "Outstanding Amount");
            this.StartDate = BasicRecord.GetDateColumn(init, "");
            this.EndDate = BasicRecord.GetDateColumn(init, "");
        }
    }
}