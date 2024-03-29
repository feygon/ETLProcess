﻿using System;
using System.Collections.Generic;
using BasicPreprocess.General;
using System.Globalization;
using BasicPreprocess.General.Interfaces;
using BasicPreprocess.General.Containers;
using System.Linq;

namespace BasicPreprocess.Specific
{
    using StringMap = Dictionary<string, string>;

    /// <summary>
    /// Container for a primary unique keyed data set reflecting Atrio's Statement File records.
    /// </summary>
    internal sealed class InvoiceData : BasicDoc, iDocType_Takes_StringMap<InvoiceData>
    {
        private const string MissingValueTag = "NULL";

        internal string
            accountNumberGroup
            , invoiceNum;
        internal decimal
            invoiceAmount
            , lowIncomeSubsidy
            , lateEnrollmentPenalty;
        internal Date
            fromDate
            , toDate;

        public InvoiceData() : base(null, new string[] { "Group Billing Acct ID", "Invoice Number" }.ToList()) { }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="doc">Source document</param>
        public InvoiceData(InvoiceData doc)
            : base(doc.headers, new string[] { "Group Billing Acct ID", "Invoice Number" }.ToList())
        {
            this.accountNumberGroup = doc.accountNumberGroup;
            this.compositeKey = doc.compositeKey;
            this.fromDate = doc.fromDate;
            this.invoiceAmount = doc.invoiceAmount;
            this.invoiceNum = doc.invoiceNum;
            this.lateEnrollmentPenalty = doc.lateEnrollmentPenalty;
            this.lowIncomeSubsidy = doc.lowIncomeSubsidy;
            this.toDate = doc.toDate;
        }

        public InvoiceData(string[] headers, StringMap data) 
            : base(headers, new string[] { "Group Billing Acct ID", "Invoice Number" }.ToList())
        {
            this.accountNumberGroup = data["Group Billing Acct ID"];
            this.invoiceNum = data["invoiceNum"];

            this.invoiceAmount = GetDecimalColumn(data, "invoiceAmount");
            this.lateEnrollmentPenalty = GetDecimalColumn(data, "lateEnrollmentPenalty");
            this.lowIncomeSubsidy = GetDecimalColumn(data, "lowIncomeSubsidy");

            this.fromDate = GetDateColumn(data, "fromDate");
            this.toDate = GetDateColumn(data, "toDate");
        }

        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface <see cref="iDocType_Takes_StringMap{DocM691_Invoice}"/></br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="headers">A string of column headers</param>
        /// <returns></returns>
        public InvoiceData GetT(StringMap stringMap, string[] headers)
        {
            return new InvoiceData(headers, stringMap);
        }

        /// <summary>
        /// Get a List of statement document lines from an array of strings.
        /// </summary>
        /// <param name="lines">Array of text lines in the format specified by Atrio as a statement file.</param>
        /// <returns></returns>
        public static HeaderSource<List<StringMap>, List<string>> ParseM691(string[] lines)
        {
            List<StringMap> tempMapList = new List<StringMap>();
            List<string> headerList = new List<string>();

            foreach (string line in lines)
            {
                StringMap docLine = new StringMap();
                docLine.Add("Group Billing Acct ID", Parse.TrimSubstring(line, 29, 9));
                headerList.Add("Group Billing Acct ID"); 
                docLine.Add("accountNumberGroup", docLine["Group Billing Acct ID"]);
                docLine.Add("AccountNumberGroup", docLine["Group Billing Acct ID"]);

                docLine.Add("Invoice Number", Parse.TrimSubstring(line, 20, 9));
                headerList.Add("Invoice Number"); 
                docLine.Add("invoiceNum", docLine["Invoice Number"]);
                docLine.Add("InvoiceNum", docLine["Invoice Number"]);

                docLine.Add("Invoice Amount", Parse.TrimSubstring(line, 47, 19));
                headerList.Add("Invoice Amount"); 
                docLine.Add("invoiceAmount", docLine["Invoice Amount"]);
                docLine.Add("InvoiceAmount", docLine["Invoice Amount"]);
                docLine.Add("InvoiceAmt", docLine["Invoice Amount"]);

                docLine.Add("Low-Income Subsidy Amount", Parse.TrimSubstring(line, 66, 19));
                headerList.Add("Low-Income Subsidy Amount"); 
                docLine.Add("Low Income Subsidy Amount", docLine["Low-Income Subsity Amount"]);
                docLine.Add("lowIncomeSubsidy", docLine["Low-Income Subsity Amount"]);
                docLine.Add("LowIncomeSubsidy", docLine["Low-Income Subsity Amount"]);

                docLine.Add("Late-Enrollment Penalty Amount", Parse.TrimSubstring(line, 85, 19));
                headerList.Add("Late-Enrollment Penalty Amount");
                docLine.Add("Late Enrollment Penalty Amount", docLine["Late-Enrollment Penalty Amount"]);
                docLine.Add("lateEnrollmentPenalty", docLine["Late-Enrollment Penalty Amount"]);
                docLine.Add("LateEnrollmentPenalty", docLine["Late-Enrollment Penalty Amount"]);

                docLine.Add("Invoice Period From Date", Parse.TrimSubstring(line, 10, 10));
                headerList.Add("Invoice Period From Date"); 
                docLine.Add("fromDate", docLine["Invoice Period From Date"]);
                docLine.Add("FromDate", docLine["Invoice Period From Date"]);
                docLine.Add("Invoice Start Date", docLine["Invoice Period From Date"]);

                docLine.Add("Invoice Period To Date", Parse.TrimSubstring(line, 10, 10));
                headerList.Add("Invoice Period To Date"); 
                docLine.Add("Invoice End Date", docLine["Invoice Period To Date"]);
                docLine.Add("toDate", docLine["Invoice Period To Date"]);
                docLine.Add("ToDate", docLine["Invoice Period To Date"]);

                tempMapList.Add(docLine);
            }
            HeaderSource<List<StringMap>, List<string>> ret =
                new HeaderSource<List<StringMap>, List<string>>(tempMapList, headerList.ToArray());

            // return type: headerSource<List<StringMap>, string>
            return ret;
        }

        private static string FilterMissingValue(string s)
            => IsMissingValue(s) ? "" : s;

        private static bool IsMissingValue(string s)
            => s.Equals(MissingValueTag, StringComparison.InvariantCultureIgnoreCase);
    }
}