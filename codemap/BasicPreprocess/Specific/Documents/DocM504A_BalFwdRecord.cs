using System;
using System.Diagnostics;
using System.Collections.Generic;
using BasicPreprocess.General.Interfaces;
using BasicPreprocess.General.Containers;

namespace BasicPreprocess.Specific
{
    using StringMap = Dictionary<string, string>;

    /// <summary>
    /// Container for a primary redundant keyed data set reflecting Atrio's balance forward records.
    /// </summary>
    internal sealed class DocM504A_BalFwdRecord : BasicDoc, iDocType_Takes_StringMap<DocM504A_BalFwdRecord>
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public static readonly string[] headers = {
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
        };

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
        public DocM504A_BalFwdRecord() : base(headers, "Account ID", keyIsUniqueIdentifier: false) { }

        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface <see cref="iDocType_Takes_StringMap{DocM504A_BalFwdRecord}"/></br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="sirNotAppearingInThisFilm">A member which is unused in this implementation.</param>
        /// <returns></returns>
        public DocM504A_BalFwdRecord GetT(StringMap stringMap, string[] sirNotAppearingInThisFilm = null)
        {
            return new DocM504A_BalFwdRecord(stringMap);
        }

        /// <summary>
        /// Constructor, takes Stringmap -- not yet implemented.
        /// </summary>
        /// <param name="init"></param>
        public DocM504A_BalFwdRecord(StringMap init)
            : base(headers, "Account ID", keyIsUniqueIdentifier: false)
        {
            this.AccountID = init["Account ID"] ?? "Bad header on AccountID in DocM504A";
            this.MemberName = init["Member Name"] ?? "Bad header on MemberName in DocM504A";
            this.OutstandingAmount = BasicDoc.GetDecimalColumn(init, "Outstanding Amount");
            this.StartDate = BasicDoc.GetDateColumn(init, "");
            this.EndDate = BasicDoc.GetDateColumn(init, "");
        }
    }
}