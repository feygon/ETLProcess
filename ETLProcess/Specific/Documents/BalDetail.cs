using ETLProcess.General.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETLProcess.General.Containers;
using ETLProcess.Specific.Documents;
using System.Data;
using ETLProcess.General.Containers.Members;
using System.ComponentModel;
using ETLProcess.General;

namespace ETLProcess.Specific.Documents
{
    internal sealed class BalDetail : IOutputDetail
    {
        string contractID = "";
        decimal outstandingAmount = 0.0m;
        public bool daysLate_30 = false
            , daysLate_60 = false
            , daysLate_90 = false
            , daysLate_120 = false;
        Date billingPeriodFromDate = new Date(1900, 1, 1)
            ,billingPeriodThruDate = new Date(1900, 1, 31);
        TimeSpan timeOverdue = new TimeSpan(0, 0, 0, 0, 0);

        public BalDetail() {
            Log.Write("BalDetail Sample instantiated, or unknown call to default constructor."); 
        }

        public BalDetail(DataRow data, Date dueDate)
        {
            try
            {
                contractID = data.Field<string>("Contract ID");
                outstandingAmount = data.Field<decimal>("Outstanding Amount");
                billingPeriodFromDate = data.Field<Date>("Billing Period From Date");
                billingPeriodThruDate = data.Field<Date>("Billing Period Thru Date");
                timeOverdue = billingPeriodThruDate.Subtract(dueDate ??= billingPeriodThruDate);
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
            if (option != null)
            {
                try
                {
                    return new BalDetail(data, (Date)option[0]);
                }
                catch (Exception err)
                {
                    throw new Exception("Bad cast type or unknown error", err);
                }
            }
            else
            {
                return new BalDetail(data, null);
            }
        }

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
            IEnumerable<BalDetail> bals = balances.Where(
                (bal) => lateCheck?.Invoke() ?? bal.daysLate_30 == true);
            return bals.Select((bal) => bal.outstandingAmount).Sum();
        }
    }
}
