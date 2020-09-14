using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ETLProcess.General.IO;

namespace ETLProcess.General.Containers
{
    /// <summary>
    /// A class of string lists with a comparator.
    /// </summary>
    public class KeyStrings : List<(string key, string header)>
    {
        /// <summary>
        /// A list of key strings in this object.
        /// </summary>
        public List<string> Keys { get { return this.Select(x => x.key).ToList(); } }
        /// <summary>
        /// A list of header strings whose keys are in this object.
        /// </summary>
        public List<string> Headers { get { return this.Select(x => x.header).ToList(); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public KeyStrings() : base() { }

        /// <summary>
        /// Deep equality comparer
        /// </summary>
        /// <param name="left">The list of strings to compare.</param>
        /// <param name="right">The list of strings to compare this to.</param>
        /// <returns></returns>
        public static bool operator ==(KeyStrings left, List<string> right)
        {
            bool same = false;
            if (left.Count != right.Count) { return same; }
            for (int i = 0; i < left.Count; i++)
            {
                if (left[i].key != right[i]) { return same; }
            }
            same = true;
            return same;
        }

        /// <summary>
        /// Deep equality comparer
        /// </summary>
        /// <param name="left">The list of strings to compare.</param>
        /// <param name="right">The list of strings to compare this to.</param>
        /// <returns></returns>
        public static bool operator !=(KeyStrings left, List<string> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Deep equality comparer
        /// </summary>
        /// <param name="left">The list of strings to compare.</param>
        /// <param name="right">The list of strings to compare this to.</param>
        /// <returns></returns>
        public static bool operator ==(KeyStrings left, KeyStrings right)
        {
            return left == right.Keys;
        }

        /// <summary>
        /// Deep inequality comparer
        /// </summary>
        /// <param name="left">The list of strings to compare.</param>
        /// <param name="right">The list of strings to compare this to.</param>
        /// <returns></returns>
        public static bool operator !=(KeyStrings left, KeyStrings right)
        {
            return left != right.Keys;
        }

        /// <summary>
        /// Deep equality comparer for string array.
        /// </summary>
        /// <param name="left">The list of strings to compare.</param>
        /// <param name="right">The array of strings to compare this to.</param>
        /// <returns></returns>
        public static bool operator ==(KeyStrings left, string[] right)
        {
            return (left == right.ToList());
        }



        /// <summary>
        /// Deep inequality comparer for string array.
        /// </summary>
        /// <param name="left">The list of strings to compare.</param>
        /// <param name="right">The array of strings to compare this to.</param>
        /// <returns></returns>
        public static bool operator !=(KeyStrings left, string[] right)
        {
            return !(left == right.ToList());
        }

        /// <summary>
        /// Equals override, same as base.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Does the KeyStrings contain these values?
        /// </summary>
        /// <param name="keys">Keys to check for</param>
        /// <param name="missing">A missing string, if any.</param>
        /// <param name="headers">Optionally, headers to check for too</param>
        /// <returns></returns>
        public bool Contains(List<string> keys, out string missing, List<string> headers = null)
        {
            missing = null;
            foreach (string key in Keys)
            {
                if (!Keys.Contains(key))
                {
                    missing = key;
                    return false;
                }
            }
            if (headers != null)
            {
                foreach (string hdr in headers)
                {
                    if (!headers.Contains(hdr))
                    {
                        missing = hdr;
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// GetHashCode, same as base.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// An expression to filter a selection by a string.
        /// </summary>
        /// <returns></returns>
        public DataRow[] Filter(DataTable table)
        {
            // checks
            foreach (string hdr in Headers)
            {
                if (!table.Columns.Contains(hdr))
                {
                    string exc1 = "KeyStrings: ";
                    for(int i = 0; i < Count; i++)
                    {
                        exc1 += "\"" + this[i] + "\"";
                        if (i + 1 != Count) { exc1 += ", "; }
                    }
                    string exc2 = "Table Columns: ";
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        exc2 += "\"" + table.Columns[i].ColumnName + "\"";
                        if (i + 1 != Count) { exc2 += ", "; }
                    }
                    Log.WriteException(string.Format("Keystring header \"{0}\" not found in table.\n" +
                        "{1}\n" +
                        "{2}"
                        , hdr, exc1, exc2));
                }
            }
            // expression builder
            string filterExpr = "";
            for (int i=0; i < Count; i++)
            {
                filterExpr += string.Format("{0} = '{1}'", this[i].key, this[i].header);
                if (i + 1 != Count)
                {
                    filterExpr += " and ";
                }
            }

            return table.Select(filterExpr);
        }
    }
}