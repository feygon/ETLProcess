using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UniversalCoreLib
{

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
            try
            {
                return (TClass)Activator.CreateInstance(type, classOptions);
            }
            catch (Exception err)
            {
                throw new Exception($"Failure creating instance {typeof(TClass).Name}:", err);
            }
        }

        /// <summary>
        /// Invoke a method on a newly created instance of T by the method's name and return its return value.
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
            if (methodOptions.Length != methodOptionTypes.Length)
            {
                throw new Exception("Options and OptionTypes are not the same length. Reflection will fail.");
            }
            try
            {
                var instance = Activator.CreateInstance(typeof(TClass), classOptions);
                MethodInfo InstanceInfo = typeof(TClass).GetMethod(methodName, methodOptionTypes);
                return (TRet)InstanceInfo.Invoke(instance, methodOptions);
            }
            catch (Exception err)
            {
                throw new Exception($"Failure invoking method \"{methodName}\" from instance of class {typeof(TClass).Name}: ", err);
            }
        }

        /// <summary>
        /// Invoke a method on a preexisting instance of T by the method's name and return its return value.
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
            if (methodOptions.Length != methodOptionTYpes.Length)
            {
                throw new Exception("Options and OptionTypes are not the same length. Reflection will fail.");
            }
            try
            {
                MethodInfo InstanceInfo = typeof(TClass).GetMethod(methodName, methodOptionTYpes);
                return (TRet)InstanceInfo.Invoke(instance, methodOptions);
            }
            catch (Exception err)
            {
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
            try
            {
                FieldInfo fieldInfo = typeof(TClass).GetField(fieldName, bindingMask);
                return (TRet)fieldInfo.GetValue(instance);
            }
            catch (Exception err)
            {
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
            try
            {
                FieldInfo fieldInfo = typeof(TClass).GetField(fieldName, bindingMask);
                return (TRet)fieldInfo.GetValue(instance);
            }
            catch (Exception err)
            {
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
            }
            catch (Exception err)
            {
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
            try
            {
                PropertyInfo propertyInfo = typeof(TClass).GetProperty(propertyName, bindingMask);
                return (TRet)propertyInfo.GetValue(instance);
            }
            catch (Exception err)
            {
                throw new Exception($"Failure creating property {propertyName} in class {typeof(TClass).Name}:", err);
            }
        } // end method
    } // end class
} // end namespace
