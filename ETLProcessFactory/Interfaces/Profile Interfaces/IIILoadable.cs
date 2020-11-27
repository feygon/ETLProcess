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
    /// Interface to promise the structure of an ETLProcess which receives records from a loadable source,
    /// i.e. an XML tree, delimited file, or SQL query.
    /// </summary>
    public interface IIILoadable
    {
        /// <summary>
        /// Step 1: Populate docs from a loadable source, i.e. an XML tree, delimited file, or SQL query<br/>
        /// Fulfill a promise that a specific implementation will have a way to populate docs using members in the client object.
        /// </summary>
        public IClient PopulateRecords();
    }
}