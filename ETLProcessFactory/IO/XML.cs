using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using ETLProcessFactory.Containers;
using ETLProcessFactory;
using ETLProcessFactory.IO;
using ETLProcessFactory.Interfaces;
using System.Data;

namespace ETLProcessFactory.IO
{
    internal sealed class XML {
        // TO DO: Export finalized statement doc.
        /// <summary>
        /// Export to xml
        /// </summary>
        /// <param name="fileNameOut">The file to export</param>
        /// <param name="outputData">The list of documents to export</param>
        public static void Export<T>(
                string fileNameOut
                , List<T> outputData 
            ) where T : ISerializable
        {
            // TO DO: Decouple OutputDoc from Export using IOutputDoc interface -> Serializable?

            // Export the XML
            Log.Write("Exporting XML");

            Stream saveOutput = File.Open($"{fileNameOut}.xml", FileMode.Create);
            var xmlWorkOrders = new XmlSerializer(typeof(List<T>));
            xmlWorkOrders.Serialize(saveOutput, outputData);
            
            using Stream outputXMLFile = File.Open($"{fileNameOut}.result", FileMode.Create);
            using TextWriter textWriter = new StreamWriter(outputXMLFile);
            textWriter.WriteLine("result=0");
        }
    }
}