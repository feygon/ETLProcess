using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ETLProcess.General.Containers;

namespace ETLProcess.General.Interfaces
{
    /// <summary>
    /// Promises that a General ETLProcess will be able to be used to generate a record.
    /// </summary>
    public interface IGeneratesRecords
    {
        /// <summary>
        /// Promise of a dictionary of bools, indicating whether a specific KeyString is unique in the interfaced class.
        /// </summary>
        public Dictionary<KeyStrings, bool> UniqueKeys_YN { get; }
        /// <summary>
        /// Promise of a counter for redundant records.
        /// </summary>
        public int RedundantRecords { get; }
    }
}