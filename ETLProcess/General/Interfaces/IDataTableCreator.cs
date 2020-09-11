using System;
using System.Collections.Generic;
using System.Data;
using ETLProcess.General.Containers;

namespace ETLProcess.General.Interfaces
{
    /// <summary>
    /// An interface for classes whose overarching purpose is to 
    ///     organize records into a DataTable.
    /// </summary>
    public interface IDataTableCreator<T>
    {
        /// <summary>
        /// A promise that this object will organize records into a dataTable,
        ///     with optional ability to create a foreign key constraint.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public DataTable GetTable(ForeignKeyConstraintElements constraint = null);
        /// <summary>
        /// A promise that this object will allow records in a dataTable to be
        ///     accessed in constant time by referencing their unique index keys,
        ///     which have been pre-populated into this Dictionary in linear time.
        /// </summary>
        public Dictionary<T, DataRow> keyedRows { get; }
    }
}