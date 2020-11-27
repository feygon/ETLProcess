using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ETLProcessFactory.Algorithms;
using ETLProcessFactory.Containers;
using ETLProcessFactory.Interfaces;
using ETLProcessFactory.Profiles;

namespace ETLProcessFactory.Containers.Dictionaries {
    /// <summary>
    /// Wrapper for a string-keyed Dictionary with ValueTuple values of Type and bool.
    /// </summary>
    public class TableHeaders : Dictionary<string, (Type colType, bool isKey)> {
        /// <summary>
        /// public parameterless constructor.
        /// </summary>
        public TableHeaders() : base() { }
        public TableHeaders(int capacity) : base(capacity) { }
        public TableHeaders(IEqualityComparer<string> comparer) : base(comparer) { }
        public TableHeaders(IDictionary<string, (Type colType, bool isKey)> dictionary) 
            : base(dictionary) { }
        public TableHeaders(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer) { }
        public TableHeaders(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public TableHeaders(
                IDictionary<string, (Type colType, bool isKey)> dictionary
                , IEqualityComparer<string> comparer)
            : base(dictionary, comparer) { }
    }

    /// <summary>
    /// Alias class for string-keyed Dictionary of strings
    /// </summary>
    public class StringMap : Dictionary<string, string>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public StringMap() : base() { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="dict"></param>

        public StringMap(Dictionary<string, string> dict) : base()
        {
            foreach (var x in dict)
            {
                Add(x.Key, x.Value);
            }
        }

    }

    /// <summary>
    /// A dictionary with auto-incrementing integer keys.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Index_Dictionary<T> : Dictionary<int, T>
    {
        /// <summary>
        /// Constructor for dictionary with auto-incrementing integer keys
        /// </summary>
        /// <param name="iList">The list to parse into a dictionary with auto-incrementing keys.</param>
        public Index_Dictionary(IList<T> iList) : base()
        {
            int x = 0;
            foreach (T item in iList)
            {
                Add(x, item);
                x++;
            }
        }
    }

    /// <summary>
    /// Modeler for multiple Index_Dictionary classes, to get matching-index values,
    ///     if ordinals are unavailable.
    /// </summary>
    public class Model_Index_Dict<T1, T2> : Dictionary<int, (T1, T2)>
    {
        /// <summary>
        /// Constructor for modeler of multiple auto-incrementing indexed classes.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public Model_Index_Dict(Index_Dictionary<T1> first, Index_Dictionary<T2> second) : base() {
            if (first.Count != second.Count)
            {
                throw new Exception("Modeled index dictionaries do not match in size.");
            } else {
                foreach (int i in first.Keys)
                {
                    Add(i, (first[i], second[i]));
                }
            }
        }

        /// <summary>
        /// Return the row keyed to the headers, in the order they appear in the lists.
        /// </summary>
        /// <typeparam name="T1s">The data type of the headers</typeparam>
        /// <typeparam name="T2s">The data type of the row data</typeparam>
        /// <param name="headers">The headers</param>
        /// <param name="row">The row of data</param>
        /// <returns></returns>
        public static Dictionary<T1s, T2s> Model_Select<T1s, T2s>(IList<T1s> headers, IList<T2s> row)
        {
            Model_Index_Dict<T1s, T2s> model = new Model_Index_Dict<T1s, T2s>(
                new Index_Dictionary<T1s>(headers),
                new Index_Dictionary<T2s>(row));
            Dictionary<T1s, T2s> ret = new Dictionary<T1s, T2s>();
            foreach (int i in model.Keys) {
                ret.Add(model[i].Item1, model[i].Item2);
            }
            return ret;
        } // end method
    } // end class
} // end namespace