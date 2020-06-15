using System;
using System.Diagnostics;
using System.Collections.Generic;
using ETLProcess.General.Interfaces;
using ETLProcess.General.Containers;
using ETLProcess.General;
using System.Linq;
using ETLProcess.General.Containers.Members;

namespace ETLProcess.Specific
{
    /// <summary>
    /// Container for a primary redundant keyed data set reflecting Client's balance forward records.
    /// </summary>
    internal sealed class BalFwdRecords : BasicDetail, IRecord<BalFwdRecords>
    {
        private static readonly List<string> Headers = new string[]{
            "Member ID"
            ,"Member Name"
            ,"Contract ID"
            ,"Account ID"
            ,"Billing Period From Date"
            ,"Billing Period Thru Date"
            ,"Outstanding Amount"
            ,"Number of Days Overdue"
        }.ToList();

        private static readonly Dictionary<string, Type> ColumnTypes = new Dictionary<string, Type>
        {
            { "Member ID", typeof(string) }
            ,{ "Member Name", typeof(string) }
            ,{ "Contract ID", typeof(string) }
            ,{ "Account ID", typeof(string) }
            ,{ "Billing Period From Date", typeof(Date) }
            ,{ "Billing Period Thru Date", typeof(Date) }
            ,{ "Outstanding Amount", typeof(decimal) }
            ,{ "Number of Days Overdue", typeof(int) }
        };

        /// <summary>
        /// Satisfies interface requirement for headers accessor to above readonly Headers member.
        /// </summary>
        public List<string> headers
        {
            get { return Headers; }
        }

        public Dictionary<string, Type> columnTypes { get { return ColumnTypes; } }

        public override List<string> GetHeaders()
        {
            return Headers;
        }

        public override Type GetChildType()
        {
            return this.GetType();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="record">Record to be copied.</param>
        public BalFwdRecords(BalFwdRecords record) : base(record)
        {
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BalFwdRecords() : base(
            data: null
            , keyIsUniqueIdentifier: false) 
        {
        }

        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface <see cref="IRecord{DocM504A_BalFwdRecord}"/></br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="sirNotAppearingInThisFilm">A member which is unused in this implementation.</param>
        /// <returns></returns>
        public BalFwdRecords Record(StringMap stringMap, List<string> sirNotAppearingInThisFilm = null)
        {
            return new BalFwdRecords(stringMap);
        }

        /// <summary>
        /// Constructor, takes Stringmap.
        /// </summary>
        /// <param name="init"></param>
        public BalFwdRecords(StringMap init)
            : base(
                  data: init
                  , keyIsUniqueIdentifier: false)
        { // as normal. 
        }
    }
}