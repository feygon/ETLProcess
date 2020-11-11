using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

using ETLProcessFactory.IO;
using ETLProcessFactory.Containers.Members;
using ETLProcessFactory.Interfaces;

namespace ETLProcess.Specific.Documents
{
	/// <summary>
	/// Client-specific class for outputting documents.
	/// </summary>
    public class OutputDoc : IOutputDoc, ISerializable
	{
		/// <summary>
		/// String member.
		/// </summary>
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
		/// <summary>
		/// Decimal member.
		/// </summary>
		public decimal
			invoiceAmount = 0.0m
			, lowIncomeSubsidyAmount = 0.0m
			, lateEnrollmentPenaltyAmount = 0.0m
			, fullBalance = 0.0m
			, lateBalance = 0.0m;
		/// <summary>
		/// DateTime member.
		/// </summary>
		public DateTime
			invoicePeriodFromDate = new DateTime(1900, 1, 1)
			, invoicePeriodToDate = new DateTime(1900, 1, 31)
			, dueDate = new DateTime(1900, 1, 1);
		/// <summary>
		/// Detail members.
		/// </summary>
		public List<BalDetail> details = new List<BalDetail>();
        readonly BalDetail sample = new BalDetail();

		/// <summary>
		/// Default constructor, for getting a sample.
		/// </summary>
		public OutputDoc() { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="statementData">A DataRow for a statement.</param>
		/// <param name="memberData">A DataRow for a member matching the statement.</param>
		/// <param name="balFwdData">A list of DataRows for balance details matching the member.</param>
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
			premiumWithhold = memberData.Field<string>("Premium Withhold");

			dueDate = ClientBusinessRules.GetDueDate(
				statementData.Field<DateTime>("Invoice Period To Date")
				, ClientBusinessRules.StatementGracePeriod);

			fullBalance = invoiceAmount;
			foreach (var detail in balFwdData)
            {
				BalDetail det = (BalDetail)sample.Record(detail, new object[] { new Date(dueDate.Year, dueDate.Month, dueDate.Day) });
				details.Add(det);
				fullBalance += det.outstandingAmount;
            }
			lateBalance = BalDetail.GetLateBalance(details.ToArray());
			
			// TO DO: implement constructor.
		}

		/// <summary>
		/// Satisfies interface serializability.
		/// </summary>
		/// <param name="info">Serialization info, such as values and types.</param>
		/// <param name="context">Streaming context, such as source and destination of serialized stream</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
			info.AddValue("groupBillingID_Statement", groupBillingAccountID_Statement);
			info.AddValue("invoiceNumber", invoiceAmount);
			info.AddValue("invoiceAmount", invoiceAmount);
			info.AddValue("lowIncomeSubsidyAmount", lowIncomeSubsidyAmount);
			info.AddValue("lateEnrollmentPenaltyAmount", lateEnrollmentPenaltyAmount);
			info.AddValue("invoicePeriodFromDate", invoicePeriodFromDate);
			info.AddValue("invoicePeriodToDate", invoicePeriodToDate);
			info.AddValue("billingAccountNumber_MemberFile", billingAccountNumber_MemberFile);
			info.AddValue("firstName", firstName);
			info.AddValue("middleName", middleName);
			info.AddValue("lastName", lastName);
			info.AddValue("address1", address1);
			info.AddValue("address2", address2);
			info.AddValue("city", city);
			info.AddValue("state", state);
			info.AddValue("zip", zip);
			info.AddValue("memberID", memberID);
			info.AddValue("premiumWithhold", premiumWithhold);
			info.AddValue("dueDate", dueDate);
			info.AddValue("fullBalance", fullBalance);
			info.AddValue("details", details, typeof(BalDetail));
			info.AddValue("lateBalance", lateBalance);
        }

		/// <summary>
		/// A sample-static builder for OutputDocs.
		/// </summary>
		/// <param name="statementData"></param>
		/// <param name="otherData"></param>
		/// <returns></returns>
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