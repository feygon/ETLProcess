using ETLProcessFactory.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using UniversalCoreLib;

namespace ETLProcessFactory.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientCore
    {
        /// <summary>
        /// Type of IClient object.
        /// </summary>
        public static Type IClientType { get; private set; }
        /// <summary>
        /// List of interface types.
        /// </summary>
        public static List<Type> interfaceTypes;
        /// <summary>
        /// Map of interface types in IClient.
        /// </summary>
        public static List<string> interfaceMethods;
        /// <summary>
        /// List of table types found in interfaces.
        /// </summary>
        public static List<Type> tableTypes;

        /// <summary>
        /// Extension method: Get full type info for the client, 
        ///   especially including client-specific generic interfaces.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static IClient GetIClientType(IClient client) { 
            IClientType = client.GetType();
            GatherInterfaceInfo();
            return client;
        }

        private static void GatherInterfaceInfo()
        {
            // Fill the type map with various important bits of information about
            //  the interfaces added to the DataSet derived class 
            //  (Boilerplate example a.k.a. "ClientETLProcess" class)
            interfaceTypes = IClientType.GetInterfaces().ToList();
            interfaceMethods = new List<string>();
            tableTypes = new List<Type>();
            foreach (Type type in interfaceTypes) {
                type.GetMethods().ToList().ForEach(
                    (method) => interfaceMethods.Add(method.Name));

                if (type.GenericTypeArguments.Length == 2
                    && Inheritance.IsDerivedFromBaseType(type.GenericTypeArguments[1], typeof(BasicRecord<>))
                    && !tableTypes.Contains(type))
                {
                    tableTypes.Add(type);
                }
            }
            
            Type[] interfaces = IClientType.GetInterfaces();
            interfaces.ToList().ForEach((type) => type.GetMethods()
                .ToList().ForEach((method) => interfaceMethods.Add(method.Name)));
        }

        #region Static Extension Methods
        /// <summary>
        /// Extension method: Export SQL Reports. Call after ProcessRecords.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static IClient ExportReports(this IClient client)
        {
            return client.ExportReports();
        }
        /// <summary>
        /// Extension method: Process document records.<br/>
        /// <Pre>Pre: Upon construction of client class, documents already retrieved from files,
        ///   imported to tables.</Pre><br/>
        /// <Post> Post: A schema has been built around the relations of these tables.</Post>
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static IClient ProcessRecords(this IClient client)
        {
            return client.ProcessRecords();
        }
        #endregion
        ///// <summary>
        ///// Extension method: Order files in the file list, in order of processing.
        ///// Typically, do the database trunk first.
        ///// </summary>
        ///// <param name="client">The client class to extend.</param>
        ///// <param name="files">The filename strings.</param>
        ///// <param name="fileListOrder">The Queue to return the order of the filename strings to.</param>
        ///// <returns></returns>
        //public static IClient OrderFileList(this IClient client, string[] files, out Queue<string> fileListOrder)
        //{
        //    if (interfaceMethods.Contains("OrderFileList")) {
        //        return client.OrderFileList(files, out fileListOrder);
        //    } else 
        //        throw new Exception("Bad call to OrderFileList method, unknown interface or missing interface IILoadable_File.");
        //}
    }
}