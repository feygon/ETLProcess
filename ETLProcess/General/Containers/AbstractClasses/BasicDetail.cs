using ETLProcess.Specific;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProcess.General.Containers
{
    /// <summary>
    /// A basicDetail has the same basic structure as a basicDoc, but its children
    ///     use a different interface.
    /// </summary>
    internal abstract class BasicDetail : BasicRecord
    {
        // Default constructor
        public BasicDetail() : base() { }
        // Copy Constructor
        public BasicDetail(BasicRecord record) : base(record) { }
        // Data Constructor
        public BasicDetail(StringMap data = null, bool keyIsUniqueIdentifier = true) 
            : base(data, keyIsUniqueIdentifier) { }

        /// <summary>
        /// Accessor for headers string list, requiring further override from grandchild.~
        /// </summary>
        /// <returns></returns>
        public override abstract List<string> GetHeaders();
        /// <summary>
        /// Child override to satisfy abstract base, requiring further overriding from grandchild.
        /// </summary>
        /// <returns></returns>
        public override abstract Type GetChildType();
    }
}
