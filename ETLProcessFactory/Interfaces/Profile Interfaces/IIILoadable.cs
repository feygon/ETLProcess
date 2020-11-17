using ETLProcessFactory.Containers;
using ETLProcessFactory.Containers.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProcessFactory.Interfaces
{
    /// <summary>
    /// Interface to promise the structure of an ETLProcess which receives files from a ZipFile.
    /// </summary>
    public interface IIILoadable
    {
        /// <summary>
        /// Promise that a specific implementation will have a way to populate docs using members in the client object.
        /// </summary>
        public void PopulateRecords();
    }
}