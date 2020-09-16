using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ETLProcess.General.Containers;
using ETLProcess.General.Interfaces;

using ETLProcess.General.Profiles;

namespace ETLProcess.General
{
    /// <summary>
    /// Wrapper for a string-keyed Dictionary with ValueTuple values of Type and bool.
    /// </summary>
    public class SampleColumnTypes : Dictionary<string, (Type colType, bool isKey)>
    {
        /// <summary>
        /// public parameterless constructor.
        /// </summary>
        public SampleColumnTypes() : base() { }
        internal SampleColumnTypes(int capacity) : base(capacity) { }
        internal SampleColumnTypes(IEqualityComparer<string> comparer) : base(comparer) { }
        internal SampleColumnTypes(IDictionary<string, (Type colType, bool isKey)> dictionary) : base(dictionary) { }
        internal SampleColumnTypes(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer) { }
        internal SampleColumnTypes(SerializationInfo info, StreamingContext context) : base(info, context) { }
        internal SampleColumnTypes(IDictionary<string, (Type colType, bool isKey)> dictionary, IEqualityComparer<string> comparer)
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

    /// <summary>
    /// A class of static methods extending the C# Reflection API.
    /// <para>Provides generic calls to GetMethod, GetField, and GetProperty.</para>
    /// </summary>
    /// <typeparam name="TClass">Type of the class to call the static method/field/property from.</typeparam>

    public static class Reflection<TClass> where TClass : class
    {
        /// <summary>
        /// A method to return an instance of type TClass, with optional parameters for its construction.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="classOptions"></param>
        /// <returns></returns>
        public static TClass GetReflectedInstance(Type type = null, object[] classOptions = null)
        {
            try {
                return (TClass)Activator.CreateInstance(type, classOptions);
            } catch (Exception err) {
                throw new Exception($"Failure creating instance {typeof(TClass).Name}:", err);
            }
        }

        /// <summary>
        /// Invoke a method on T by its name and return its return value.
        /// </summary>
        /// <typeparam name="TRet">Return type of the method.</typeparam>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="classOptions">Optional parameters used to instantiate the class.</param>
        /// <param name="methodOptions">Optional parameters used in the static call.</param>
        /// <param name="methodOptionTypes">Types of the optional parameters used in this method.</param>
        /// <returns></returns>
        public static TRet GetReflectedMethod<TRet>(
            string methodName
            , object[] classOptions = null
            , object[] methodOptions = null
            , Type[] methodOptionTypes = null)
        {
            if (methodOptions.Length != methodOptionTypes.Length) {
                throw new Exception("Options and OptionTypes are not the same length. Reflection will fail.");
            }
            try {
                var instance = Activator.CreateInstance(typeof(TClass), classOptions);
                MethodInfo InstanceInfo = typeof(TClass).GetMethod(methodName, methodOptionTypes);
                return (TRet)InstanceInfo.Invoke(instance, methodOptions);
            }
            catch (Exception err) {
                throw new Exception($"Failure invoking method \"{methodName}\" from instance of class {typeof(TClass).Name}: ", err);
            }
        }

        /// <summary>
        /// Invoke a method on T by its name and return its return value.
        /// </summary>
        /// <typeparam name="TRet">Return type of the method.</typeparam>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="instance">Instance of the object to call this on.</param>
        /// <param name="methodOptions">Optional parameters used in the static call.</param>
        /// <param name="methodOptionTYpes">Types of the optional parameters used in this method.</param>
        /// <returns></returns>
        public static TRet GetReflectedMethod<TRet>(
            string methodName
            , TClass instance
            , object[] methodOptions = null
            , Type[] methodOptionTYpes = null)
        {
            if (methodOptions.Length != methodOptionTYpes.Length) {
                throw new Exception("Options and OptionTypes are not the same length. Reflection will fail.");
            }
            try {
                MethodInfo InstanceInfo = typeof(TClass).GetMethod(methodName, methodOptionTYpes);
                return (TRet)InstanceInfo.Invoke(instance, methodOptions);
            } catch (Exception err) {
                throw new Exception($"Failure invoking method \"{methodName}\" from instance of class {typeof(TClass).Name}:", err);
            }
        }

        /// <summary>
        /// Return a field value from T by its name.
        /// </summary>
        /// <typeparam name="TRet">Return type of the field.</typeparam>
        /// <param name="fieldName"></param>
        /// <param name="classOptions">Optional parameters used to instantiate the class.</param>
        /// <param name="bindingMask">Bitmask of BindingFlags values, reflecting the static/public/etc. attributes of this field.</param>
        /// <returns></returns>
        public static TRet GetReflectedField<TRet>(
            string fieldName
            , object[] classOptions = null
            , BindingFlags bindingMask = 0)
        {
            var instance = Activator.CreateInstance(typeof(TClass), classOptions);
            try {
                FieldInfo fieldInfo = typeof(TClass).GetField(fieldName, bindingMask);
                return (TRet)fieldInfo.GetValue(instance);
            } catch (Exception err) {
                throw new Exception($"Failure creating field {fieldName} in class {typeof(TClass).Name}:", err);
            }
        }

        /// <summary>
        /// Return a field value from T by its name.
        /// </summary>
        /// <typeparam name="TRet">Return type of the field.</typeparam>
        /// <param name="fieldName"></param>
        /// <param name="instance">Instance of the object to call this on.</param>
        /// <param name="bindingMask">Bitmask of BindingFlags values, reflecting the static/public/etc. attributes of this field.</param>
        /// <returns></returns>
        public static TRet GetReflectedField<TRet>(
            string fieldName
            , TClass instance
            , BindingFlags bindingMask = 0)
        {
            try {
                FieldInfo fieldInfo = typeof(TClass).GetField(fieldName, bindingMask);
                return (TRet)fieldInfo.GetValue(instance);
            } catch (Exception err) {
                throw new Exception($"Failure creating field {fieldName} in class {typeof(TClass).Name}:", err);
            }
        }

        /// <summary>
        /// Return a property value from T by its name.
        /// </summary>
        /// <typeparam name="TRet">Return type of the property.</typeparam>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="classOptions">Optional parameters used to instantiate the class.</param>
        /// <param name="bindingMask">Bitmask of BindingFlags values, reflecting the static/public/etc. attributes of this property.</param>
        /// <returns></returns>
        public static TRet GetReflectedProperty<TRet>(
            string propertyName
            , object[] classOptions = null
            , BindingFlags bindingMask = 0)
        {
            try
            {
                var instance = Activator.CreateInstance(typeof(TClass), classOptions);
                PropertyInfo propertyInfo = typeof(TClass).GetProperty(propertyName, bindingMask);
                return (TRet)propertyInfo.GetValue(instance);
            } catch (Exception err) {
                throw new Exception($"Failure creating property {propertyName} in class {typeof(TClass).Name}:", err);
            }
        }

        /// <summary>
        /// Return a property value from T by its name.
        /// </summary>
        /// <typeparam name="TRet">Return type of the property.</typeparam>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="instance">Instance of the object to call this on.</param>
        /// <param name="bindingMask">Bitmask of BindingFlags values, reflecting the static/public/etc. attributes of this property.</param>
        /// <returns></returns>
        public static TRet GetReflectedProperty<TRet>(
            string propertyName
            , TClass instance
            , BindingFlags bindingMask = 0)
        {
            try {
                PropertyInfo propertyInfo = typeof(TClass).GetProperty(propertyName, bindingMask);
                return (TRet)propertyInfo.GetValue(instance);
            } catch (Exception err) {
                throw new Exception($"Failure creating property {propertyName} in class {typeof(TClass).Name}:", err);
            }
        } // end method
    } // end class
    
    /// <summary>
    /// Class for extending LinQ on Dataset capabilities.
    /// </summary>
    public class LinQOnDataset
    {
        /// <summary>
        /// Method to get the remainder of Left sans Intersection of Left/Right
        /// </summary>
        /// <param name="leftTable">The left table in the join</param>
        /// <param name="leftKey">The key to the left table in the join (composite allowed, must mirror rightKey in length and types)</param>
        /// <param name="rightTable">The right table in the join</param>
        /// <param name="rightKey">The key to the right table in the join (composite allowed, must mirror leftKey in length and types)</param>
        /// <returns></returns>
        public static EnumerableRowCollection<DataRow> Left_NotInner(
            DataTable leftTable
            , DataColumn[] leftKey
            , DataTable rightTable
            , DataColumn[] rightKey)
        {
            EnumerableRowCollection<DataRow> query = leftTable.AsEnumerable();

            // Check that all keys match, and keep those that do.
            for (int i = 0; i < leftKey.Length; i++)
            {
                query = from left in query
                        where !(from right in rightTable.AsEnumerable()
                                select right[rightKey[i]]
                                ).Contains(left[leftKey[i]])
                        select left;

            }
            return query;
        }
        /// <summary>
        /// Method to get the left rows of a left join of left/right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="relation"></param>
        /// <param name="right"></param>
        /// <param name="columns">Optional: The columns to query, left joined with the left table (requires reformulation of new types of DataRows). Null: All columns from the left table.</param>
        /// <typeparam name="TBasicRecordLeft"></typeparam>
        /// <typeparam name="TBasicRecordRight"></typeparam>
        /// <typeparam name="TProfile"></typeparam>
        /// <returns></returns>
        public static EnumerableRowCollection<DataRow> LeftJoin_LeftRows<TBasicRecordLeft, TBasicRecordRight, TProfile>(
            FileDataRecords<TBasicRecordLeft, TProfile> left
            , FileDataRecords<TBasicRecordRight, TProfile> right
            , DataRelation relation
            , DataColumn[] columns = null)
            where TBasicRecordLeft : BasicRecord<TBasicRecordLeft>, IRecord<TBasicRecordLeft>, new()
            where TBasicRecordRight : BasicRecord<TBasicRecordRight>, IRecord<TBasicRecordRight>, new()
            where TProfile : IC_CSVFileIn<IO_FilesIn>, new()
        {



            if (columns == null) {
                EnumerableRowCollection<DataRow> query = left.AsEnumerable();
                query = from lt in left.AsEnumerable()
                        join rt in right.AsEnumerable()
                        on new {  } equals new { relation.ParentColumns }
                        select lt;
            } else { throw new NotImplementedException("Not impemented: 'Requires reformulation of new types of DataRows with according columns'"); }
        }

        /// <summary>
        /// Method to get the DataSet of two left joined tables.
        /// </summary>
        /// <typeparam name="TBasicRecordLeft"></typeparam>
        /// <typeparam name="TBasicRecordRight"></typeparam>
        /// <typeparam name="TProfile"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public static DataSet LeftJoin_DataSet<TBasicRecordLeft, TBasicRecordRight, TProfile>(
            FileDataRecords<TBasicRecordLeft, TProfile> left
            , FileDataRecords<TBasicRecordRight, TProfile> right
            , DataRelation relation)
            where TBasicRecordLeft : BasicRecord<TBasicRecordLeft>, IRecord<TBasicRecordLeft>, new()
            where TBasicRecordRight : BasicRecord<TBasicRecordRight>, IRecord<TBasicRecordRight>, new()
            where TProfile : IC_CSVFileIn<IO_FilesIn>, new()
        {
            var childColColl = relation.ChildColumns.AsEnumerable();
            var parentColColl = relation.ParentColumns.AsEnumerable();

            var innerOnly = left.AsEnumerable().Join(
                right.AsEnumerable()
                , lKey => from x in childColColl select left.Columns[x.ColumnName]
                , rKey => from y in parentColColl select right.Columns[y.ColumnName]
                , (lKey, rKey) => lKey).DefaultIfEmpty();
            throw new NotImplementedException("To do: 'https://stackoverflow.com/questions/584820/how-do-you-perform-a-left-outer-join-using-linq-extension-methods' and 'https://stackoverflow.com/questions/10317117/linq-to-sql-join-on-multiple-columns-using-lambda'");

                /*
                from lt in left.AsEnumerable()
                join rt in right.AsEnumerable()
                on new { lt.Field(relation.ChildColumns[0].ColumnName) } equals new { } into all
                from allX in all.DefaultIfEmpty()
                select allX;
            */

        }
    }
} // end namespace