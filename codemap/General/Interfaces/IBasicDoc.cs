using BasicPreprocess.General.Containers;
using System.Data;

namespace codemap.General.Containers
{
    /// <summary>
    /// Interface for <see cref="BasicDoc"/>.
    /// </summary>
    public interface IBasicDoc
    {
        /// <summary>
        /// The table this doc's data row will go into.
        /// </summary>
        DataTable ParentTable { get; set; }
        /// <summary>
        /// The row this doc's fields will go into.
        /// </summary>
        DataRow DocRow { get; set; }
    }
}