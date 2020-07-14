using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETLProcess.General.Interfaces;
using ETLProcess.General.Containers;
using ETLProcess.General;
using System.Reflection;
using System.Data;

using ETLProcess.General.Containers.Members;

namespace ETLProcess.Specific.Documents
{
	internal sealed class OutputDoc : IOutputDoc
	{
		public Address mailingAddress, returnAddress;
		public string accountNumber, message, statementDate, dueDate;
		public decimal balanceDue, currentBalance, fullBalance, agingFullPastDue, aging30, aging60, aging90, aging120;
		public List<IOutputDetail> _details;
		
		/// <summary>
		/// Default constructor, for getting a sample.
		/// </summary>
		public OutputDoc()
		{
			this.mailingAddress = new Address();
			this.returnAddress = new Address();
			this.accountNumber = "";
			this.statementDate = "";
			this.balanceDue = 0.0m;
			this._details = new List<IOutputDetail>();
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

		public OutputDoc(
			DataRow docRow)
		{

			// TO DO: implement constructor.
			throw new NotImplementedException();
		}

		public IOutputDoc Record(
			DataRow data)
		{
			return new OutputDoc(data);
		}
	}
}