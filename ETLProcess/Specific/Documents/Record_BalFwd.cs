using System;
using System.Diagnostics;
using System.Collections.Generic;
using ETLProcess.General.Interfaces;
using ETLProcess.General.Containers;
using ETLProcess.General;
using System.Linq;
using ETLProcess.General.Containers.Members;
using System.Runtime.CompilerServices;

using ETLProcess.General.Containers.AbstractClasses;

namespace ETLProcess.Specific
{
    /// <summary>
    /// Container for a primary redundant keyed data set reflecting Client's balance forward records.
    /// </summary>
    internal sealed class Record_BalFwd : BasicRecord<Record_BalFwd>, IRecord<Record_BalFwd>
    {
        public SampleColumnTypes columnTypes { get; } = new SampleColumnTypes
        {
            { "Member ID", (typeof(string), false) }
            ,{ "Member Name", (typeof(string), false) }
            ,{ "Contract ID", (typeof(string), false) }
            ,{ "Account ID", (typeof(string), true) }
            ,{ "Billing Period From Date", (typeof(Date), false) }
            ,{ "Billing Period Thru Date", (typeof(Date), false) }
            ,{ "Outstanding Amount", (typeof(decimal), false) }
            ,{ "Number of Days Overdue", (typeof(int), false) }
        };

        /// <summary>
        /// Satisfies interface requirement for headers accessor to above readonly Headers member.
        /// </summary>
        public List<string> headers { get { return columnTypes.Keys.ToList(); } }

        public override List<string> GetHeaders()
        {
            return headers;
        }

        public override Type GetChildType()
        {
            return this.GetType();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="record">Record to be copied.</param>
        public Record_BalFwd(Record_BalFwd record) : base(record)
        {

        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Record_BalFwd() : base(){ }

        /// <summary>
        /// Constructor, takes Stringmap.
        /// </summary>
        /// <param name="init"></param>
        /// <param name="sampleColumnTypes">A dictionary of column types by header name in this type of Record</param>
        public Record_BalFwd(StringMap init, SampleColumnTypes sampleColumnTypes)
            : base(
                  data: init
                  , sampleColumnTypes
                  , keyIsUniqueIdentifier: false)
        { // as normal. 
        }


        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface <see cref="IRecord{DocM504A_BalFwdRecord}"/></br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="sampleColumnTypes">A dictionary of column types by header name in this type of Record</param>
        /// <param name="sirNotAppearingInThisFilm">A member which is unused in this implementation.</param>
        /// <returns></returns>
        public Record_BalFwd Record(
            StringMap stringMap
            , SampleColumnTypes sampleColumnTypes
            , List<string> sirNotAppearingInThisFilm = null)
        {
            return new Record_BalFwd(stringMap, sampleColumnTypes);
        }
    }
}