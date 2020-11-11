using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ETLProcessFactory.Containers;
using System.Runtime.Serialization;

namespace ETLProcessFactory.Interfaces
{
    /// <summary>
    /// Interface to make promises about output documents
    /// </summary>
    public interface IOutputDoc : ISerializable
    {
        /// <summary>
        /// Promise that an output document will be able to be recorded from a DataRow.
        /// </summary>
        /// <param name="data">The row to be output as a document.</param>
        /// <param name="obj">Optional parameters.</param>
        /// <returns></returns>
        public IOutputDoc Record(DataRow data, object[] obj = null);
    }
}