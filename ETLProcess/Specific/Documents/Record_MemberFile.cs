using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ETLProcess.General.Interfaces;
using ETLProcess.General.Containers;
using ETLProcess.General;

namespace ETLProcess.Specific
{
    /// <summary>
    /// Container for a primary unique keyed set of data reflecting Client's member records.
    /// </summary>
    internal sealed class MemberRecords : BasicRecord, IRecord<MemberRecords>
    {
        private static readonly List<string> Headers = new string[] {
            "Billing Account Number"
            , "First Name"
            , "Middle Name"
            , "Last Name"
            , "Address1"
            , "Address2"
            , "City"
            , "State"
            , "Zip"
            , "MemberID"
            , "Premium Withhold"
        }.ToList();

        /// <summary>
        /// Satisfies interface requirement for headers accessor to above readonly Headers member.
        /// </summary>
        public List<string> headers {
            get { return Headers; }
        }

        private readonly Dictionary<string, Type> ColumnTypes = new Dictionary<string, Type>()
        {
            { "Billing Account Number", typeof(string) }
            , { "First Name", typeof(string) }
            , { "Middle Name", typeof(string) }
            , { "Last Name", typeof(string) }
            , { "Address1", typeof(string) }
            , { "Address2", typeof(string) }
            , { "City", typeof(string) }
            , { "State", typeof(string) }
            , { "Zip", typeof(string) }
            , { "MemberID", typeof(string) }
            , { "Premium Withhold", typeof(string) }
        };
        public Dictionary<string, Type> columnTypes { get { return ColumnTypes; } }

        public override List<string> GetHeaders() {
            return Headers;
        }

        public override Type GetChildType() {
            return this.GetType();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="record">Record to be copied.</param>
        public MemberRecords(MemberRecords record) : base(record) 
        {
        }

        public MemberRecords() : base(
            data:null
            , keyIsUniqueIdentifier: true)
        {
        }

        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface</br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="headers">The column headers</param>
        /// <returns></returns>
        public MemberRecords Record(StringMap stringMap, List<string> headers)
        {
            return new MemberRecords(stringMap, headers);
        }

        /// <summary>
        /// Constructor that takes a StringMap and headers.
        /// <br>Required by the IRecord interface.</br>
        /// </summary>
        /// <param name="memberFile">StringMap of a line of data</param>
        /// <param name="headers">Headers of the data</param>
        public MemberRecords(StringMap memberFile, List<string> headers) 
            : base(
                  data:memberFile
                  , keyIsUniqueIdentifier: true)
        {

            foreach (string header in headers)
            {
                if (!Headers.Contains(header)) { 
                    string temp = "CSV Header \"" + header + "\" not found in Member Records.";
                    throw new Exception(temp);
                }
            }
        }
    }
}
