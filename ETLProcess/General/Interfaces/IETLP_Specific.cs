using ETLProcess.General.Containers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleColumnTypes = System.Collections.Generic.Dictionary<string, System.Type>;

namespace ETLProcess.General.Interfaces
{


    /// <summary>
    /// Interface to promise the structure of an ETLProcess which receives files from a ZipFile.
    /// </summary>
    public interface IETLP_Specific<T_IETLP>
    {
        /// <summary>
        /// Promise that a specific implementation will have a way to populate docs using members in the client object.
        /// </summary>
        public void PopulateRecords();

        /// <summary>
        /// Promise that an implementation of this interface will have access
        /// to the names of columns containing primary keys, and the types of those columns.
        /// </summary>
        public Dictionary<Type, SampleColumnTypes> SampleColumns { get; }
    }
}
