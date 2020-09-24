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
using System.Runtime.InteropServices;
using ETLProcess.General.Algorithms;
using System.Data.Metadata.Edm;

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
        internal SampleColumnTypes(IDictionary<string, (Type colType, bool isKey)> dictionary) 
            : base(dictionary) { }
        internal SampleColumnTypes(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer) { }
        internal SampleColumnTypes(SerializationInfo info, StreamingContext context) : base(info, context) { }
        internal SampleColumnTypes(
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
            foreach (int i in model.Keys) {
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
} // end namespace