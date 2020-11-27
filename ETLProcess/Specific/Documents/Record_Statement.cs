using System;
using System.Linq;
using System.Collections.Generic;

using ETLProcessFactory;
using ETLProcessFactory.Algorithms;
using ETLProcessFactory.Interfaces;
using ETLProcessFactory.Containers;
using ETLProcessFactory.Containers.Members;
using ETLProcessFactory.Containers.AbstractClasses;
using ETLProcessFactory.Containers.Dictionaries;
using System.Data;

namespace ETLProcess.Specific
{
    /// <summary>
    /// Container for a primary unique keyed data set reflecting Client's Statement File records.
    /// </summary>
    public class Record_Statement : 
        BasicRecord<Record_Statement>
        , IRecord<Record_Statement>
        , IRecord_Uses_ImportRows<Record_Statement>
    {

        private static TableHeaders _columnTypes = new TableHeaders {
                { "Group Billing Acct ID", (typeof(string), true) }
                ,{ "Invoice Number", (typeof(string), true) }
                ,{ "Invoice Amount", (typeof(decimal), false) }
                ,{ "Low-Income Subsidy Amount", (typeof(decimal), false) }
                ,{ "Late-Enrollment Penalty Amount", (typeof(decimal), false) }
                ,{ "Invoice Period From Date", (typeof(DateTime), false) }
                ,{ "Invoice Period To Date", (typeof(DateTime), false) }
        };

        /// <summary>
        /// Collection of column types and boolean whether they're part of the primary key.
        /// </summary>
        public TableHeaders ColumnTypes { get { return _columnTypes; } }

        /// <summary>
        /// Satisfies interface requirement for headers accessor to above readonly Headers member.
        /// </summary>
        public List<string> Headers { get { return ColumnTypes.Keys.ToList(); } }

        /// <summary>
        /// Get the headers of this row type.
        /// </summary>
        public override List<string> GetHeaders() {
            return Headers;
        }

        /// <summary>
        /// Get the type of the child class.
        /// </summary>
        public override Type GetChildType() {
            return this.GetType();
        }

        /// <summary>
        /// Default constructor, for samples and XMLSerializer only.
        /// </summary>
        public Record_Statement() : base(){ keyIsUniqueIdentifier = true; }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="record">Source document</param>
        public Record_Statement(Record_Statement record)
            : base(record)
        {
            keyIsUniqueIdentifier = true;
        }

        /// <summary>
        /// /// Constructor that takes a StringMap and headers.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="table">Table constructing this record's DataRow</param>
        /// <param name="headers">Headers of the data</param>
        /// <param name="sampleColumnTypes">A dictionary of column types by header name in this type of Record</param>
        public Record_Statement(
            StringMap data
            , DataTable table
            , TableHeaders sampleColumnTypes
            , List<string> headers) 
            : base(
                  data: data
                  , table
                  , sampleColumnTypes
                  , keyIsUniqueIdentifier: true)
        {
            foreach (string header in headers) {
                if (!this.Headers.Contains(header))
                {
                    string temp = "CSV Header \"" + header + "\" not found in Statement Records.";
                    throw new Exception(temp);
                }
            }
            // base is a Dictionary<string, string>
        }

        /// <summary>
        /// A method that calls a Constructor which takes a StringMap
        /// <br>Satisfies interface</br>
        /// </summary>
        /// <param name="stringMap">The stringmap to have turned into a Balance Forward record.</param>
        /// <param name="table">Table constructing this record's DataRow</param>
        /// <param name="sampleColumnTypes">A dictionary of column types by header name in this type of Record</param>
        /// <param name="headers">A string of column headers</param>
        /// <returns></returns>
        public Record_Statement Record(
            StringMap stringMap
            , DataTable table
            , TableHeaders sampleColumnTypes
            , List<string> headers)
        {
            return new Record_Statement(stringMap, table, sampleColumnTypes, headers);
        }

        /// <summary>
        /// Get a List of statement document lines from an array of strings.
        /// </summary>
        /// <param name="lines">Array of text lines in the format specified by Client as a statement file.</param>
        /// <returns></returns>
        public HeaderSource<List<StringMap>, List<string>> ParseRows(string[] lines)
        {
            List<StringMap> tempMapList = new List<StringMap>();

            foreach (string line in lines)
            {
                StringMap docLine = new StringMap
                {
                    { "Group Billing Acct ID", Parse.TrimSubstring(line, 29, 9) },
                    { "Invoice Number", Parse.TrimSubstring(line, 20, 9) },
                    { "Invoice Amount", Parse.TrimSubstring(line, 47, 19) },
                    { "Low-Income Subsidy Amount", Parse.TrimSubstring(line, 66, 19) },
                    { "Late-Enrollment Penalty Amount", Parse.TrimSubstring(line, 85, 19) },
                    { "Invoice Period From Date", Parse.TrimSubstring(line, 0, 10) },
                    { "Invoice Period To Date", Parse.TrimSubstring(line, 10, 10) }
                };

                tempMapList.Add(docLine);
            }
            HeaderSource<List<StringMap>, List<string>> ret =
                new HeaderSource<List<StringMap>, List<string>>(tempMapList, Headers.ToArray());

            // return type: headerSource<List<StringMap>, string>
            return ret;
        }
    }
}