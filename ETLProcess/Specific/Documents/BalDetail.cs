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
        string contractID = null;
        decimal outstandingAmount = 0.0m;
        Date billingPeriodFromDate = new Date(1900, 1, 1)
            ,billingPeriodThruDate = new Date(1900, 1, 31);
        int daysOverdue = -42;

        public BalDetail() {
            Log.Write("BalDetail Sample instantiated, or unknown call to default constructor."); 
        }

        public BalDetail(DataRow data)
        {
            try
            {
                contractID = data.Field<string>("Contract ID");
                outstandingAmount = data.Field<decimal>("Outstanding Amount");
                billingPeriodFromDate = data.Field<Date>("Billing Period From Date");
                billingPeriodThruDate = data.Field<Date>("Billing Period Thru Date");
                daysOverdue = data.Field<int>("Number of Days Overdue");
            } catch (System.Exception err) {
                throw new Exception("Bad header name if IndexOutOfRangeException, wrong type if InvalidCastException, or unknown exception", err);
            }
        }

        /// <summary>
        /// A sample-static builder for DetailOutputDocs.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Use return 'as DetailOutputDoc'</returns>
        public IOutputDoc Record(DataRow data)
        {
            return new BalDetail(data);
        }

        public Type GetChildType()
        {
            return GetType();
        }
    }
}
