using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicPreprocess.General.Interfaces;
using BasicPreprocess.General.Containers;

namespace BasicPreprocess.Specific.Documents
{
	using StringMap = Dictionary<string, string>;
	internal sealed class OutputDoc : BasicRecord, IRecord<OutputDoc>
    {
        public Address mailingAddress, returnAddress;
        public string statementID, accountNumber, message, statementDate, dueDate;
        public decimal balanceDue, currentBalance, fullBalance, agingFullPastDue, aging30, aging60, aging90, aging120;
        public Dictionary<string, IRecord<BasicRecord>> details; // string, based on accountNumber type.
		public List<BasicDetail> _details;
		public OutputDoc() : base(null, new string[] { "accountNumber" }.ToList())
		{
			this.mailingAddress = new Address();
			this.returnAddress = new Address();
			this.accountNumber = "";
			this.statementDate = "";
			this.balanceDue = 0.0m;
			this.details = new Dictionary<string, IRecord<BasicRecord>>();
			this.message = "";
			this.currentBalance = 0.0m;
			this.aging30 = 0.0m;
			this.aging60 = 0.0m;
			this.aging90 = 0.0m;
			this.agingFullPastDue = 0.00m;
			this.dueDate = "";
			this.aging120 = 0.0m;
			this.fullBalance = 0.0m;
		}

		public OutputDoc(List<string> headers, StringMap data) : base(headers, new string[] { "accountNumber" }.ToList())
		{
			// TO DO: Implement boilerplate example.
		}

		public OutputDoc GetRecord(StringMap stringMap, List<string> headers)
		{
			return new OutputDoc(headers, stringMap);
		}
	}
}
