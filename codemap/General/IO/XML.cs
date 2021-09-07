using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using static BasicPreprocess.Parse;
using BasicPreprocess.General.Containers;
using BasicPreprocess.General;
using BasicPreprocess.General.IO;
using BasicPreprocess.General.Interfaces;

namespace BasicPreprocess.General.IO
{
    internal sealed class XML
    {
        // TO DO: Export finalized statement doc to xml.
        /// <summary>
        /// Export to xml
        /// </summary>
        /// <param name="fileName">The file to export</param>
        /// <param name="documents">The list of documents to export</param>
        /// <param name="Debug">The log where debug messages will go.</param>
        public static void Export<TBasicDoc>(
                string fileName,
                KeyedDocs<TBasicDoc> documents,
                ILog Debug
            ) where TBasicDoc : BasicDoc, iDocType_Takes_StringMap<TBasicDoc>, new()
        {
            // Export the XML
            Debug.Write("Exporting XML");
#if DEBUG
            using (Stream saveWorkOrders = File.Open($"{fileName}.xml", FileMode.Create))
#else
            using (Stream saveWorkOrders = File.Open(fileName, FileMode.Create))
#endif
            {
                // Save Work Orders
                var xmlWorkOrders = new XmlSerializer(typeof(List<TBasicDoc>));
                xmlWorkOrders.Serialize(saveWorkOrders, documents);
            }
            // Create the Uluro result file
            FileStream fileStream = File.Open($"{fileName}.result", FileMode.Create);
            Stream saveResultFile = fileStream;
            StreamWriter streamWriter = new StreamWriter(saveResultFile);
            TextWriter resultWriter = streamWriter;
            resultWriter.WriteLine("result=0");
        }
    }
}
