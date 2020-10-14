using System;
using System.Data;

namespace ETLProcess.General.ExtendLinQ
{
    /// <summary>
    /// A DataTable with self-hashing rows.
    /// </summary>
    public class DataTable_Hashing: DataTable
    {
        /// <summary>
        /// Constructor for a DataTable with self-hashing rows.
        /// </summary>
        public DataTable_Hashing() : base()
        {
            throw new NotImplementedException("Need to implement all overrides referring to DataRow, with DataRow_Hashing, including ISerializable and XMLSerializationInfo");
        }

        
    }
}
