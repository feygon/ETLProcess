using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicPreprocess.General.Interfaces;
using BasicPreprocess.General.Containers;

namespace BasicPreprocess.Specific
{
    using StringMap = Dictionary<string, string>;

    /// <summary>
    /// Container for a primary unique keyed set of data reflecting Atrio's member records.
    /// </summary>
    internal sealed class DocM690_MemberRecord : BasicDoc, iDocType_Takes_StringMap<DocM690_MemberRecord>
    {
        private const string MissingValueTag = "NULL";
        public string
            MemberID
            , BillingAcctNum
            , PremiumWithhold;
        public Address address;

        public DocM690_MemberRecord() : base(null, "Billing Account Number") { }

        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface <see cref="iDocType_Takes_StringMap{DocM690_MemberRecord}"/></br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="headers">The column headers</param>
        /// <returns></returns>
        public DocM690_MemberRecord GetT(StringMap stringMap, string[] headers)
        {
            return new DocM690_MemberRecord(stringMap, headers);
        }

        /// <summary>
        /// Constructor that takes a StringMap and headers.
        /// <br>Required by the GetT interface.</br>
        /// <br><see cref="iDocType_Takes_StringMap{DocM690_MemberRecord}"/></br>
        /// </summary>
        /// <param name="memberFile">StringMap of a line of data</param>
        /// <param name="headers">Headers of the data</param>
        public DocM690_MemberRecord(Dictionary<string, string> memberFile, string[] headers) 
            : base(headers, "Billing Account Number")
        {
            // TO DO: check headers?

            string firstName = memberFile["First Name"];
            firstName = IsMissingValue(firstName) ? "" : $"{firstName} ";
            string middleName = memberFile["Middle Name"];
            middleName = IsMissingValue(middleName) ? "" : $"{middleName} ";
            string lastName = FilterMissingValue(memberFile["Last Name"]);

            address = new Address
            {
                name = $"{firstName}{lastName}",
                line1 = FilterMissingValue(memberFile["Address1"]),
                line2 = FilterMissingValue(memberFile["Address2"]),
                city = FilterMissingValue(memberFile["City"]),
                state = FilterMissingValue(memberFile["State"]),
                zip = FilterMissingValue(memberFile["Zip"])
            };
            this.MemberID = memberFile["MemberID"];        // check this.
            this.BillingAcctNum = memberFile["Billing Account Number"]; // check this.
            this.PremiumWithhold = memberFile["Premium Withold"]; // check this.
        }

        private static string FilterMissingValue(string s)
    => IsMissingValue(s) ? "" : s;

        private static bool IsMissingValue(string s)
            => s.Equals(MissingValueTag, StringComparison.InvariantCultureIgnoreCase);
    }
}
