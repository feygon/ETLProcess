using ETLProcess.General.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleColumnTypes = System.Collections.Generic.Dictionary<string, System.Type>;

namespace ETLProcess.General.Interfaces
{
    /// <summary>
    /// Promise that a class will implement methods specific to taking in CSV data.
    /// </summary>
    public interface I_CSVIn
    {
        /// <summary>
        /// Accessor for key column names of any CSV classes that may be involved in the process, accessible by type.
        /// </summary>
        public Dictionary<Type, SampleColumnTypes> SampleColumns { get; } // should be explicit, so it's accessible publicly by interfacing child class.

        ///// <summary>
        ///// Accessor for key column of any CSV classes that may be involved in the process, accessible by a master sample instance.
        ///// </summary>
        //public Dictionary<BasicRecord, (List<string>, List<Type>)> CSVSampleColumns { get; }
    }
}
