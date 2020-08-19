using ETLProcess.General.Containers;
using ETLProcess.General.Containers.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProcess.General.Interfaces
{
    /// <summary>
    /// Interface to promise the structure of an ETLProcess which receives files from a ZipFile.
    /// </summary>
    public interface IA_InputProfile
    {
        /// <summary>
        /// Promise that a specific implementation will have a way to populate docs using members in the client object.
        /// </summary>
        public void PopulateRecords();
    }
}