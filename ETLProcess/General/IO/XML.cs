using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ETLProcess.General.Containers;
using ETLProcess.General;
using ETLProcess.General.IO;
using ETLProcess.General.Interfaces;

namespace ETLProcess.General.IO
{
    internal sealed class XML
    {
        // TO DO: Export finalized statement doc.
        /// <summary>
        /// Export to xml
        /// </summary>
        /// <param name="fileName">The file to export</param>
        /// <param name="documents">The list of documents to export</param>
        public static void Export<TOutputDoc>(
                string fileName
                , List<IOutputDoc> documents
            ) where TOutputDoc : IOutputDoc, new()
        {
            // Export the XML
            Log.Write("Exporting XML");
#if DEBUG
            using (Stream saveWorkOrders = File.Open($"{fileName}.xml", FileMode.Create))
#else
            using (Stream saveWorkOrders = File.Open(fileName, FileMode.Create))
#endif
            {
                // Save Work Orders
                var xmlWorkOrders = new XmlSerializer(typeof(List<TOutputDoc>));
                xmlWorkOrders.Serialize(saveWorkOrders, documents);
            }
            // Create the Uluro result file
            using Stream saveResultFile = File.Open($"{fileName}.result", FileMode.Create);
            using TextWriter resultWriter = new StreamWriter(saveResultFile);
            resultWriter.WriteLine("result=0");
        }
    }
}
