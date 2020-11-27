using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Serialization;

using ETLProcessFactory.Interfaces;
using ETLProcessFactory.Containers;
using ETLProcessFactory.Containers.Members;
using ETLProcessFactory.IO;
using ETLProcessFactory;
using ETLProcess.Specific.Documents;
using ETLProcessFactory.Containers.Dictionaries;
using UniversalCoreLib;

namespace ETLProcess.Specific.Documents
{
    /// <summary>
    /// Output class of balane details.
    /// </summary>
    public class BalDetail : IOutputDetail
    {
        // TO DO: Document better.
        /// <summary>
        /// Member
        /// </summary>
        public string memberID = "";
        /// <summary>
        /// Member
        /// </summary>
        public string contractID = "";
        /// <summary>
        /// Member
        /// </summary>
        public decimal outstandingAmount = 0.0m;
        /// <summary>
        /// Member
        /// </summary>
        public bool daysLate_30 = false
            , daysLate_60 = false
            , daysLate_90 = false
            , daysLate_120 = false;
        /// <summary>
        /// Member
        /// </summary>
        public DateTime billingPeriodFromDate = new DateTime(1900, 1, 1)
            , billingPeriodThruDate = new DateTime(1900, 1, 31);
        /// <summary>
        /// Member
        /// </summary>
        public TimeSpan timeOverdue = new TimeSpan(0, 0, 0, 0, 0);

        /// <summary>
        /// Balance Details
        /// </summary>
        public BalDetail() {
            Log.Write("BalDetail Sample instantiated, or unknown call to default constructor."); 
        }

        /// <summary>
        /// Constructor with parameters (use this!)
        /// </summary>
        /// <param name="data">Row data, for constructing from.</param>
        /// <param name="dueDate">Due date of the appropriate statement (can be null)</param>
        public BalDetail(DataRow data, Date dueDate)
        {
            try
            {
                memberID = data.Field<string>("Member ID");
                contractID = data.Field<string>("Contract ID");
                outstandingAmount = data.Field<decimal>("Outstanding Amount");
                billingPeriodFromDate = data.Field<DateTime>("Billing Period From Date");
                billingPeriodThruDate = data.Field<DateTime>("Billing Period Thru Date");
                timeOverdue = billingPeriodThruDate.Subtract(dueDate 
                    ??= (Date)billingPeriodThruDate.AddDays(ClientBusinessRules.StatementGracePeriod));
                daysLate_30 = timeOverdue.Days >= 30;
                daysLate_60 = timeOverdue.Days >= 60;
                daysLate_90 = timeOverdue.Days >= 90;
                daysLate_120 = timeOverdue.Days >= 120;
            } catch (System.Exception err) {
                throw new Exception("Bad header name if IndexOutOfRangeException, wrong type if InvalidCastException, or unknown exception", err);
            }
        }

        /// <summary>
        /// A sample-static builder for DetailOutputDocs.
        /// </summary>
        /// <param name="data">The balance forward table datarow to be added as a detail.</param>
        /// <param name="option">Index 0 as Date: invoice due date.</param>
        /// <returns>Use return 'as DetailOutputDoc'</returns>
        public IOutputDoc Record(DataRow data, object[] option = null)
        {
            if (option != null) {
                try {
                    return new BalDetail(data, (Date)option[0]);
                } catch (Exception err) {
                    throw new Exception("Bad cast type or unknown error", err);
                }
            } else {
                return new BalDetail(data, null);
            }
        }
        /// <summary>
        /// Get the type of the child.
        /// </summary>
        /// <returns></returns>
        public Type GetChildType()
        {
            return GetType();
        }

        /// <summary>
        /// Get the total of balances forward which are past due by 30 days or more.
        /// </summary>
        /// <param name="balances">A list of balances forward</param>
        /// <param name="lateCheck">Optional lambda for checking if a balance is late.</param>
        /// <returns></returns>
        public static decimal GetLateBalance(BalDetail[] balances, DelRet<bool> lateCheck = null)
        {
            IEnumerable<BalDetail> lateBals = balances.Where(
                (bal) => lateCheck?.Invoke() ?? bal.daysLate_30 == true);
            return lateBals.Select((bal) => bal.outstandingAmount).Sum();
        }

        /// <summary>
        /// Satisfies interface serializability.
        /// </summary>
        /// <param name="info">Serialization info, such as values and types.</param>
        /// <param name="context">Streaming context, such as source and destination of serialized stream</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("memberID", memberID);
            info.AddValue("contractID", contractID);
            info.AddValue("billingPeriodFromDate", billingPeriodFromDate);
            info.AddValue("billingPeriodThruDate", billingPeriodThruDate);
            info.AddValue("daysLate_120", daysLate_120);
            info.AddValue("daysLate_30", daysLate_30);
            info.AddValue("daysLate_60", daysLate_60);
            info.AddValue("daysLate_90", daysLate_90);
            info.AddValue("outstandingAmount", outstandingAmount);
            info.AddValue("timeOverdue", timeOverdue);
        }
    }
}
