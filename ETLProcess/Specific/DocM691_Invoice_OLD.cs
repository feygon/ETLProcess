using System;
using System.Collections.Generic;
using BasicETLProcess.General;

namespace BasicETLProcess
{
	internal sealed class DocM691_Invoice : BasicDoc
	{
        private const string MissingValueTag = "NULL";

        public Address mailingAddress;
		public string 
            accountNumber
            , memberID
            , message
            , statementDate
            , fullName
            , dueDate
            , premiumWithhold;
        public decimal 
            balanceDue
            , currentBalance
            , agingFullPastDue;
        public List<Detail_M691> details;

        public DocM691_Invoice() : base(null, "Group Billing Acct ID", "Invoice Number") { }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="doc">Source document</param>
        public DocM691_Invoice(DocM691_Invoice doc)
            : base(doc.headers, "Group Billing Acct ID", "Invoice Number")
        {
            this.mailingAddress = doc.mailingAddress;
            this.accountNumber = doc.accountNumber;
            this.memberID = doc.memberID;
            this.message = doc.message;
            this.statementDate = doc.statementDate;
            this.fullName = doc.statementDate;
            this.dueDate = doc.dueDate;
            this.premiumWithhold = doc.premiumWithhold;
            this.balanceDue = doc.balanceDue;
            this.currentBalance = doc.currentBalance;
            this.agingFullPastDue = doc.agingFullPastDue;
            this.details = doc.CopyOfDetails();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="headers">An array of headers as strings</param>
        /// <param name="init">The data to be ingested.</param>
        public DocM691_Invoice(string[] headers, IDictionary<string, string> init) 
            : base(headers, "Group Billing Acct ID", "Invoice Number")
		{
            string firstName = init["First Name"];
            firstName = IsMissingValue(firstName) ? "" : $"{firstName} ";
            string middleName = init["Middle Name"];
            middleName = IsMissingValue(middleName) ? "" : $"{middleName} ";
            string lastName = FilterMissingValue(init["Last Name"]);
            mailingAddress = new Address
            {
                name = $"{firstName}{lastName}",
                line1 = FilterMissingValue(init["Address1"]),
                line2 = FilterMissingValue(init["Address2"]),
                city = FilterMissingValue(init["City"]),
                state = FilterMissingValue(init["State"]),
                zip = FilterMissingValue(init["Zip"])
            };
            accountNumber = init["Billing Account Number"];
            memberID = init["MemberID"];
            statementDate = "";
            fullName = $"{firstName}{middleName}{lastName}";
            message = "";
            currentBalance = 0.0m;
            agingFullPastDue = 0.00m;
            dueDate = "";
            premiumWithhold = init["Premium Withhold"];
            details = new List<Detail_M691>();
            
        }

        /// <summary>
        /// Iteratively call copy constructor of details.
        /// </summary>
        /// <returns></returns>
        public List<Detail_M691> CopyOfDetails()
        {
            List<Detail_M691> ret = new List<Detail_M691>();
            foreach (Detail_M691 det in this.details)
            {
                Detail_M691 detCopy = new Detail_M691(det);
                ret.Add(detCopy);
            }
            return ret;
        }

        /// <summary>
        /// Get account number from detail.
        /// </summary>
        /// <param name="index">Index of detail to get account number from.</param>
        public void IngestDetail691(int index)
        {
            Detail_M691 detail = details[index];
            this.accountNumber = detail.accountNumberGroup;
        }

        private static string FilterMissingValue(string s)
            => IsMissingValue(s) ? "" : s;

        private static bool IsMissingValue(string s)
            => s.Equals(MissingValueTag, StringComparison.InvariantCultureIgnoreCase);
    }
}
