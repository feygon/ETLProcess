using ETLProcess.General.Containers.Members;
using ETLProcess.General.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ETLProcess.General.IO;

namespace ETLProcess.Specific.Documents
{
    internal sealed class OutputDoc : IOutputDoc
	{

		public string
			groupBillingAccountID_Statement = ""
			, invoiceNumber = ""
			, billingAccountNumber_MemberFile = ""
			, firstName = ""
			, middleName = ""
			, lastName = ""
			, address1 = ""
			, address2 = ""
			, city = ""
			, state = ""
			, zip = ""
			, memberID = ""
			, premiumWithhold = "";
		public decimal
			invoiceAmount = 0.0m
			, lowIncomeSubsidyAmount = 0.0m
			, lateEnrollmentPenaltyAmount = 0.0m
			, fullBalance = 0.0m
			, lateBalance = 0.0m;
		public DateTime
			invoicePeriodFromDate = new DateTime(1900, 1, 1)
			, invoicePeriodToDate = new DateTime(1900, 1, 31)
			, dueDate = new DateTime(1900, 1, 1);
		public List<BalDetail> details = new List<BalDetail>();
		BalDetail sample = new BalDetail();

		// public Address mailingAddress, returnAddress;
		// public string accountNumber, message;
		// Date statementStartDate, statementEndDate, dueDate;
		// public decimal invoiceAmt, currentBalance, fullBalance;
		// public int daysPastDue;
		// public List<IOutputDetail> _details;

		/// <summary>
		/// Default constructor, for getting a sample.
		/// </summary>
		public OutputDoc() { }

		public OutputDoc(
			DataRow statementData
			, DataRow memberData
			, List<DataRow> balFwdData = null)
		{
			// statement items
			groupBillingAccountID_Statement = statementData.Field<string>("Group Billing Acct ID");
			invoiceNumber = statementData.Field<string>("Invoice Number");
			invoiceAmount = statementData.Field<decimal>("Invoice Amount");
			lowIncomeSubsidyAmount = statementData.Field<decimal>("Low-Income Subsidy Amount");
			lateEnrollmentPenaltyAmount = statementData.Field<decimal>("Late-Enrollment Penalty Amount");
			invoicePeriodFromDate = statementData.Field<DateTime>("Invoice Period From Date");
			invoicePeriodToDate = statementData.Field<DateTime>("Invoice Period To Date");

			// member file items
			billingAccountNumber_MemberFile = memberData.Field<string>("Billing Account Number");
			firstName = memberData.Field<string>("First Name");
			middleName = memberData.Field<string>("Middle Name");
			lastName = memberData.Field<string>("Last Name");
			address1 = memberData.Field<string>("Address1");
			address2 = memberData.Field<string>("Address2");
			city = memberData.Field<string>("City");
			state = memberData.Field<string>("State");
			zip = memberData.Field<string>("Zip");
			memberID = memberData.Field<string>("MemberID");
			premiumWithhold = memberData.Field<string>("Premium Withold");

			dueDate = ClientBusinessRules.GetDueDate(
				statementData.Field<Date>("Billing Period Thru Date")
				, ClientBusinessRules.StatementGracePeriod);

			fullBalance = invoiceAmount;
			foreach (var detail in balFwdData)
            {
				BalDetail det = (BalDetail)sample.Record(detail, new object[] { dueDate });
				details.Add(det);
				fullBalance += det.outstandingAmount;
            }
			lateBalance = BalDetail.GetLateBalance(details.ToArray());
			
			// TO DO: implement constructor.
			throw new NotImplementedException();
		}

		public IOutputDoc Record(
			DataRow statementData, object[] otherData = null)
		{
			if (otherData != null)
            {
				try
                {
					return new OutputDoc(statementData, (DataRow)otherData[0], (List<DataRow>)otherData[1]);
				} catch (Exception err) {
					Log.WriteException("Bad otherData types or unknown error: ", err);
					return null;
                }
            } else {
				return null;
			}
		}
	}
}