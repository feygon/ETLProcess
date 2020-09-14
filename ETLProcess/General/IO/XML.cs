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
using ETLProcess.Specific.Documents;

namespace ETLProcess.General.IO
{
    internal sealed class XML
    {
        // TO DO: Export finalized statement doc.
        /// <summary>
        /// Export to xml
        /// </summary>
        /// <param name="fileNameOut">The file to export</param>
        /// <param name="outputData">The list of documents to export</param>
        public static void Export(
                string fileNameOut
                , List<OutputDoc> outputData
            )
        {
            // TO DO: Decouple OutputDoc from Export using IOutputDoc interface -> Serializable?

            // Export the XML
            Log.Write("Exporting XML");

            Stream saveOutput = File.Open($"{fileNameOut}.xml", FileMode.Create);
            var xmlWorkOrders = new XmlSerializer(typeof(List<OutputDoc>));
            xmlWorkOrders.Serialize(saveOutput, outputData);
            
            using Stream outputXMLFile = File.Open($"{fileNameOut}.result", FileMode.Create);
            using TextWriter textWriter = new StreamWriter(outputXMLFile);
            textWriter.WriteLine("result=0");
        }
    }
}
