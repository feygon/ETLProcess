using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ETLProcessFactory.Interfaces;
using ETLProcessFactory.Containers;
using ETLProcessFactory;
using ETLProcessFactory.Containers.AbstractClasses;
using ETLProcessFactory.GP;

namespace ETLProcess.Specific
{
    /// <summary>
    /// Container for a primary unique keyed set of data reflecting Client's member records.
    /// </summary>
    public class Record_Members : BasicRecord<Record_Members>, IRecord<Record_Members>
    {
        /// <summary>
        /// Container for a primary unique keyed data set reflecting Client's Statement File records.
        /// </summary>
        public TableHeaders ColumnTypes { get; } = new TableHeaders()
        {
            { "Billing Account Number", (typeof(string), true) }
            , { "First Name", (typeof(string), false)}
            , { "Middle Name", (typeof(string), false)}
            , { "Last Name", (typeof(string), false)}
            , { "Address1", (typeof(string), false)}
            , { "Address2", (typeof(string), false)}
            , { "City", (typeof(string), false)}
            , { "State", (typeof(string), false)}
            , { "Zip", (typeof(string), false)}
            , { "MemberID", (typeof(string), false)}
            , { "Premium Withhold", (typeof(string), false)}
        };

        /// <summary>
        /// Satisfies interface requirement for headers accessor to above readonly Headers member.
        /// </summary>
        public List<string> Headers { get { return ColumnTypes.Keys.ToList(); } }

        /// <summary>
        /// Satisfies interface requirement for headers accessor to above readonly Headers member.
        /// </summary>
        public override List<string> GetHeaders() {
            return Headers;
        }

        /// <summary>
        /// Get the type of the child class.
        /// </summary>
        public override Type GetChildType() {
            return this.GetType();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="record">Record to be copied.</param>
        public Record_Members(Record_Members record) : base(record) 
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Record_Members() : base(){ }

        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface</br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="sampleColumnTypes">A dictionary of column types by header name in this type of Record</param>
        /// <param name="headers">The column headers</param>
        /// <returns></returns>
        public Record_Members Record(
            StringMap stringMap
            , TableHeaders sampleColumnTypes
            , List<string> headers)
        {
            return new Record_Members(stringMap, sampleColumnTypes, headers);
        }

        /// <summary>
        /// Constructor that takes a StringMap and headers.
        /// <br>Required by the IRecord interface.</br>
        /// </summary>
        /// <param name="memberFile">StringMap of a line of data</param>
        /// <param name="sampleColumnTypes">A dictionary of column types by header name in this type of Record</param>
        /// <param name="headers">Headers of the data</param>
        public Record_Members(
            StringMap memberFile
            , TableHeaders sampleColumnTypes
            , List<string> headers) 
            : base(
                  data:memberFile
                  , sampleColumnTypes
                  , keyIsUniqueIdentifier: true)
        {
            foreach (string header in headers)
            {
                if (!this.Headers.Contains(header)) { 
                    string temp = "CSV Header \"" + header + "\" not found in Member Records.";
                    throw new Exception(temp);
                }
            }
        }
    }
}
