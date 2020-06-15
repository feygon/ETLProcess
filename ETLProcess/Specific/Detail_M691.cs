using System;
using System.Globalization;
using System.Collections.Generic;
using BasicETLProcess.Specific;

namespace BasicETLProcess.Specific
{
    internal sealed class Detail_M691
    {
        public string accountNumberGroup;
        public string invoiceNum;
        public decimal balanceForward;
        public decimal balanceDue;
        public decimal lowIncomeSubsidy;
        public decimal lateEnrollmentPenalty;
        public DateTime fromDate;
        public DateTime toDate;

        public Detail_M691()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="det">Source detail</param>
        public Detail_M691(Detail_M691 det)
        {
            this.accountNumberGroup = det.accountNumberGroup;
            this.balanceDue = det.balanceDue;
            this.balanceForward = det.balanceForward;
            this.fromDate = det.fromDate;
            this.invoiceNum = det.invoiceNum;
            this.lateEnrollmentPenalty = det.lateEnrollmentPenalty;
            this.lowIncomeSubsidy = det.lowIncomeSubsidy;
            this.toDate = det.toDate;
        }

        public static Detail_M691 Detail691(string init)
        {
            // This is a file of current charges
            DateTime startDate = DateTime.ParseExact(Parse.TrimSubstring(init, 0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string currentMonth = startDate.ToString("MMMM", CultureInfo.InvariantCulture);
            decimal lowIncome = Parse.DecimalParse(Parse.TrimSubstring(init, 66, 19));
            decimal lateEnrollment = Parse.DecimalParse(Parse.TrimSubstring(init, 85, 19));
            return new Detail_M691 {
                fromDate = startDate,
                toDate = DateTime.ParseExact(Parse.TrimSubstring(init, 10, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                invoiceNum = Parse.TrimSubstring(init, 20, 9),
                accountNumberGroup = Parse.TrimSubstring(init, 29, 9),
                balanceForward = 0,
                balanceDue = Parse.DecimalParse(Parse.TrimSubstring(init, 47, 19)),
                lowIncomeSubsidy = lowIncome,
                lateEnrollmentPenalty = lateEnrollment,
            };
        }

        public static Detail_M691 Detail504A(DocM504A_BalFwdRecord init)
        {
            // TODO!!
            return new Detail_M691();
        }
    }
}
