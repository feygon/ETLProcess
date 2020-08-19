using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ETLProcess.General.Containers;
using ETLProcess.General;
using ETLProcess.General.IO;
using ETLProcess.General.Interfaces;
using ETLProcess.Specific.Boilerplate;
using System.Data;

namespace ETLProcess.General.IO
{
    internal sealed class XML
    {
        // TO DO: Export finalized statement doc.
        /// <summary>
        /// Export to xml
        /// </summary>
        /// <param name="fileNameOut">The file to export</param>
        /// <param name="processData">The list of documents to export</param>
        public static void Export(
                string fileNameOut
                , ClientETLProcess processData
            )
        {
            // Export the XML
            Log.Write("Exporting XML");
            using (Stream saveWorkOrders = File.Open($"{fileNameOut}.xml", FileMode.Create))
            {
                // Save Work Orders
                var xmlWorkOrders = new XmlSerializer(typeof(ClientETLProcess));
                xmlWorkOrders.Serialize(saveWorkOrders, processData);
            }
            // Create the result file
            using Stream saveResultFile = File.Open($"{fileNameOut}.result", FileMode.Create);
            using TextWriter resultWriter = new StreamWriter(saveResultFile);
            resultWriter.WriteLine("result=0");
        }
    }
}
