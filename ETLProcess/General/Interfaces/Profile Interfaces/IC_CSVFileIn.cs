﻿using ETLProcess.General.Containers;
using ETLProcess.General.Containers.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Data;

namespace ETLProcess.General.Interfaces
{
    /// <summary>
    /// Promise that a class will implement methods specific to taking in CSV data files.
    /// </summary>
    public interface IC_CSVFileIn<AbstClass> : IB_FilesIn
    {
        /// <summary>
        /// Accessor for key column names of any CSV classes that may be involved in the process, accessible by type.
        /// </summary>
        public Dictionary<Type, SampleColumnTypes> SampleColumns { get; } // should be explicit, so it's accessible publicly by interfacing child class.
        /// <summary>
        /// Promise of a bucketmap granting easy access to Tables by 2D (Type)X(int) index, instead of by their unique ID strings.
        /// </summary>
        public Dictionary<Type, List<DataTable>> TablesByType { get; }
    }
}
