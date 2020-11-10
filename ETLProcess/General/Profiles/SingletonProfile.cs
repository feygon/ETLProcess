using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;

using ETLProcess.General.Profiles;
using ETLProcess.General.IO;

namespace ETLProcess.General.Containers.AbstractClasses
{
    /// <summary>
    /// A class for lazy generic reflection of the Client's ETLProcess class,
    ///     allowing access to its members by interface identification in decoupled classes,
    ///     provided that it has been instantiated in a coupled class first.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonProfile<T> where T : class, IDisposable
    {
        // private static readonly object padlock = new object();
        private static bool childFirstRun = true;

        /// <summary>
        /// Is this object the base of the first constructed child class instance or the singleton's?
        /// </summary>
        protected bool firstRun = true;
        /// <summary>
        /// Is this object from the first constructed instance or the singleton?
        /// </summary>
        /// <summary>
        /// Object for Dispose method to dispose of.
        /// </summary>
        protected Component disposable = new Component();
        /// <summary>
        /// Was the firstRun created through the use of the Static Initializer?
        /// </summary>
        protected static bool initialized = false;

        /// <summary>
        /// Dictionary of instances by Type the non-static member is instantiated with.
        /// <para>Note: may require thread-safe access and mutation.</para>
        /// </summary>
        public static ConcurrentDictionary<Type, (object Instance, object[] classOptions)> InstanceDict { get; } =
            new ConcurrentDictionary<Type, (object Instance, object[] classOptions)>();

        /// <summary>
        /// Firstrun constructor.
        /// </summary>
        /// <param name="derivedClassType">Class Type of the derived class to be lazily instantiated.</param>
        /// <param name="classOptions">Optional parameters for the lazily instantiated class instance.</param>
        public SingletonProfile(Type derivedClassType, object[] classOptions) {
            if (!initialized) {
                Log.WriteException($"SingletonProfile derived class \"{typeof(T).Name}\" was constructed without the use of the Initializer \"{typeof(T).Name}.Init(...)\". This is unsafe. Please use the initializer.");
            }
            if (derivedClassType.BaseType.Name != typeof(SingletonProfile<T>).Name) {
                Log.WriteException($"Class type \"{derivedClassType.Name}\" baseType \"{derivedClassType.BaseType.Name}\" mismatch. " +
                    $"Must be \"{typeof(SingletonProfile<T>).Name}\" for type safety.");
            }

            firstRun = childFirstRun;
            childFirstRun = false;

            if (firstRun) {
                bool success =
                InstanceDict.TryAdd(derivedClassType,
                    (CreateInstanceOfDerivedClass(derivedClassType, classOptions)
                    , classOptions));
                if (success) {
                    Log.Write($"Type \"{derivedClassType.Name}\" added to Singleton's Dictionary of Instances.");
                } else {
                    Log.WriteException($"TryAdd failure: Derived class type \"{derivedClassType.Name}\" may already be accounted for in the Singleton's Dictionary of Instances.");
                }
            }
        }

        /// <summary>
        /// Get an instance from a member of this class.
        /// </summary>
        /// <returns></returns>
        public T GetInstance() { return (T)InstanceDict[typeof(T)].Instance ?? throw new Exception("Instance is null."); }
        /// <summary>
        /// Get the derived instance T by class statically, by class type.
        /// </summary>
        /// <returns></returns>
        public static T GetDerivedInstance() {
            bool success = InstanceDict.TryGetValue(typeof(T), out (object Instance, object[] classOptions) obj);
            if (success)
            {
                return (T)obj.Instance;
            } else { throw new NullReferenceException($"Type {typeof(T).Name} not present in Singleton's Dictionary of Instance."); }
        }

        /// <summary>
        /// Create an instance of the subclass with optional parameters and return it.
        /// </summary>
        /// <param name="derivedType">Type of the derived class to be lazily instantiated.</param>
        /// <param name="options">Optional parameters. See <see cref="Activator.CreateInstance(Type, object[])"/> on how to use this.</param>
        /// <returns></returns>
        public static T CreateInstanceOfDerivedClass(Type derivedType, object[] options = null)
        {
            return (T)Activator.CreateInstance(derivedType, options);
        }
        /// <summary>
        /// Initializer for first-run-discarded singleton classes.
        /// </summary>
        /// <param name="classOptions"></param>
        public static void Init(object[] classOptions) {
            try
            {
                if (!initialized)
                {
                    initialized = true;

                    using T discard = (T)Activator.CreateInstance(typeof(T), classOptions);
                    Log.Write($"Initializing class \"{typeof(T).Name}\" and garbage collecting first-run instance.");
                }
            } catch (Exception err) {
#if Debug
                Log.WriteException($"Unknown error while disposing of Initial class instance in derived class T: {typeof(T).FullName}.", err);
#else
                throw err;
#endif
            }
        }
    }
}