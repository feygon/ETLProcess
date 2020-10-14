using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ETLProcess.General.Containers.AbstractClasses;
using ETLProcess.General.Interfaces;
using ETLProcess.General.IO;

namespace ETLProcess.General.Profiles
{
    
    // See Linq to XML: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/linq-to-xml-overview
    /// <summary>
    /// Output profile for exporting to an XML file.
    /// </summary>
    public class IO_XMLOut : SingletonProfile<IO_XMLOut>, IDisposable {
        /// <summary>
        /// Name of the XML file to be exported as part of this output profile.
        /// </summary>
        public readonly string fileNameOut;
        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public IO_XMLOut() : base(typeof(IO_XMLOut), null) {
            this.fileNameOut = "out";
        }
        /// <summary>
        /// Constructor with string parameter for filename out.
        /// </summary>
        /// <param name="outArg"></param>
        public IO_XMLOut(string outArg) : base(typeof(IO_XMLOut), null) {
            this.fileNameOut = outArg;
        }
        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public void Dispose() {
            if (firstRun) {
                disposable.Dispose();
                GC.SuppressFinalize(this);
            } else {
                Log.WriteException("Dispose called on singleton or non-initial instance.");
            }
        } // end method

        /// <summary>
        /// Export a list of serializable items to XML for serialization into a document.
        /// </summary>
        /// <param name="outputDocs">A list of serializable output docs.</param>
        /// <param name="nonDefaultFilename">A nonstandard filename, such as for XML reports separate from the main output file.</param>
        public void Export<T>(List<T> outputDocs, string nonDefaultFilename = null) where T: ISerializable
        {
            XML.Export(nonDefaultFilename ?? fileNameOut, outputDocs);
        } // end method.
    } // end class
} // end namespace