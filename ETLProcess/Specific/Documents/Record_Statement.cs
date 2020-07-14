using System;
using System.Linq;
using System.Collections.Generic;
using ETLProcess.General;
using System.Globalization;
using ETLProcess.General.Interfaces;
using ETLProcess.General.Containers;
using System.Runtime.Remoting.Messaging;
using ETLProcess.General.Containers.Members;

namespace ETLProcess.Specific
{
    using KVP = KeyValuePair<string, Type>;
    /// <summary>
    /// Container for a primary unique keyed data set reflecting Client's Statement File records.
    /// </summary>
    internal sealed class Record_Statement : BasicRecord<Record_Statement>, IRecord<Record_Statement>, IRecord_Uses_ImportRows<Record_Statement>
    {
        public Dictionary<string, Type> columnTypes { get; } = new Dictionary<string, Type>{
            { "Group Billing Acct ID", typeof(string) }
            ,{ "Invoice Number", typeof(string) }
            ,{ "Invoice Amount", typeof(decimal) }
            ,{ "Low-Income Subsidy Amount", typeof(decimal) }
            ,{ "Late-Enrollment Penalty Amount", typeof(decimal) }
            ,{ "Invoice Period From Date", typeof(Date) }
            ,{ "Invoice Period To Date", typeof(Date) }
        };

        /// <summary>
        /// Satisfies interface requirement for headers accessor to above readonly Headers member.
        /// </summary>
        public List<string> headers { get { return columnTypes.Keys.ToList(); } }

        public override List<string> GetHeaders()
        {
            return headers;
        }

        public override Type GetChildType()
        {
            return this.GetType();
        }

        /// <summary>
        /// Default constructor, for samples and XMLSerializer only.
        /// </summary>
        public Record_Statement() : base(
            data: null
            ) 
        {
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="record">Source document</param>
        public Record_Statement(Record_Statement record)
            : base(record)
        {
            foreach (KeyValuePair<string, string> cell in record)
            {
                Add(cell.Key, cell.Value);
            }
        }

        /// <summary>
        /// /// Constructor that takes a StringMap and headers.
        /// </summary>
        /// <param name="headers">Headers of the data</param>
        /// <param name="data"></param>
        public Record_Statement(StringMap data, List<string> headers) 
            : base(
                  data: data
                  , keyIsUniqueIdentifier: true)
        {
            foreach (string header in headers) {
                if (!this.headers.Contains(header))
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
        /// <param name="headers">A string of column headers</param>
        /// <returns></returns>
        public Record_Statement Record(StringMap stringMap, List<string> headers)
        {
            return new Record_Statement(stringMap, headers);
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
                new HeaderSource<List<StringMap>, List<string>>(tempMapList, headers.ToArray());

            // return type: headerSource<List<StringMap>, string>
            return ret;
        }
    }
}