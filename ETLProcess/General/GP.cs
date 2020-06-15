using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ETLProcess.General
{
    /// <summary>
    /// Alias class for string-keyed Dictionary of strings
    /// </summary>
    public class StringMap : Dictionary<string, string> 
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public StringMap() : base() { }

    }

    /// <summary>
    /// a delegate that takes any type and doesn't return anything.
    /// </summary>
    /// <typeparam name="T1">The type (including tuples)</typeparam>
    /// <param name="t1">The type instance to be passed.</param>
    public delegate void DelVoid<T1>(T1 t1);

    /// <summary>
    /// A delegate that takes any type and returns the other type.
    /// </summary>
    /// <typeparam name="T0">The return type.</typeparam>
    /// <typeparam name="T1">The passed type.</typeparam>
    /// <param name="t1">The passed instance.</param>
    /// <returns>Returns an instance of the return type.</returns>
    public delegate T0 DelRet<T0, T1>(T1 t1);
    /// <summary>
    /// A delegate returns any type.
    /// </summary>
    /// <typeparam name="T0">The return type.</typeparam>
    /// <returns></returns>
    public delegate T0 DelRet<T0>();

    /// <summary>
    /// A delegate that takes any type and returns the other type.
    /// </summary>
    /// <typeparam name="T0">The return type.</typeparam>
    /// <typeparam name="T1">The passed array type.</typeparam>
    /// <param name="t1">The passed instance.</param>
    /// <returns>Returns an instance of the return type.</returns>
    public delegate T0 DelRetArray<T0, T1>(T1[] t1);
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
    /// Modeler for multiple Index_Dictionary classes, to get matching-index values.
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
            foreach (int i in model.Keys)
            {
                ret.Add(model[i].Item1, model[i].Item2);
            }
            return ret;
        }
    }
}
