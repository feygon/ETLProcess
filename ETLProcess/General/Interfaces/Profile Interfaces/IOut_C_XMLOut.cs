using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using ETLProcess.General.Containers.AbstractClasses;
using ETLProcess.General.Profiles;

namespace ETLProcess.General.Interfaces.Profile_Interfaces
{
    /// <summary>
    /// Promise that a class will implement methods specific to outputting XML data files.
    /// </summary>
    public interface IOut_C_XMLOut<Singleton, TOutput> : IOut_B_Files 
        where Singleton : SingletonProfile<IO_XMLOut>
        where TOutput : ISerializable
    {
        /// <summary>
        /// Promise that a specific implementation will have methods to export to XML 
        ///     from a list of serializable outputdocs of generic type T.
        /// </summary>
        /// <param name="outputDocs">List of serializable docs.</param>
        public void XMLExport(List<TOutput> outputDocs);
    }
}