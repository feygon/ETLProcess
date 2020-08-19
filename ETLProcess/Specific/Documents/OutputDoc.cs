using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

using ETLProcess.General;
using ETLProcess.General.Interfaces;
using ETLProcess.General.Containers;
using ETLProcess.General.Containers.Members;

namespace ETLProcess.Specific.Documents
{
	internal sealed class OutputDoc : IOutputDoc
	{
		public Address mailingAddress, returnAddress;
		public string accountNumber, message;
		Date statementStartDate, statementEndDate, dueDate;
		public decimal invoiceAmt, currentBalance, fullBalance;
		public int daysPastDue;
		public List<IOutputDetail> _details;
		
		/// <summary>
		/// Default constructor, for getting a sample.
		/// </summary>
		public OutputDoc()
		{
			this.mailingAddress = new Address();
			this.returnAddress = new Address();
			this.accountNumber = "";
			this.statementStartDate = null;
			this.statementEndDate = null;
			this.daysPastDue = 0;
			this.invoiceAmt = 0.0m;
			this._details = new List<IOutputDetail>();
			this.message = "";
			this.currentBalance = 0.0m;
			this.dueDate = null;
			this.fullBalance = 0.0m;
		}

		public OutputDoc(
			DataRow data)
		{
			
			mailingAddress = data.Field<Address>("Mailing Address");
			returnAddress = data.Field<Address>("Return Address");
			accountNumber = data.Field<string>("Account ID");
			statementStartDate = data.Field<Date>("Invoice Period From Date");
			statementEndDate = data.Field<Date>("Invoice Period To Date");
			dueDate = ClientBusinessRules.GetDueDate(data.Field<Date>("Billing Period Thru Date")
				     ,ClientBusinessRules.StatementGracePeriod);
			

			message = data.Field<string>("Message");
			invoiceAmt = data.Field<decimal>("Invoice Amount");

			currentBalance = data.Field<decimal>("Current Balance");
			fullBalance = data.Field<decimal>("Full Balance");
			
			// TO DO: implement constructor.
			throw new NotImplementedException();
		}

		public IOutputDoc Record(
			DataRow data, object[] sirNotAppearingInThisClass = null)
		{
			return new OutputDoc(data);
		}
	}
}