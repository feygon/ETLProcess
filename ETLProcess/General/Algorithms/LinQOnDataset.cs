using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ETLProcess.General.Algorithms
{
    /// <summary>
    /// Class for extending LinQ on Dataset capabilities.
    /// </summary>
    public class LinQOnDataset
    {
        /// <summary>
        /// Method to get the remainder of Left sans Intersection of Left/Right
        /// </summary>
        /// <param name="leftTable">The left table in the join</param>
        /// <param name="leftKey">The key to the left table in the join (composite allowed, must mirror rightKey in length and types)</param>
        /// <param name="rightTable">The right table in the join</param>
        /// <param name="rightKey">The key to the right table in the join (composite allowed, must mirror leftKey in length and types)</param>
        /// <returns></returns>
        public EnumerableRowCollection<DataRow> Left_NotInner(
            DataTable leftTable
            , DataColumn[] leftKey
            , DataTable rightTable
            , DataColumn[] rightKey)
        {
            EnumerableRowCollection<DataRow> query = leftTable.AsEnumerable();

            // Check that all keys match, and keep those that do.
            for (int i=0; i < leftKey.Length; i++)
            {
                query = from left in query
                        where !(from right in rightTable.AsEnumerable()
                                select right[rightKey[i]]
                                ).Contains(left[leftKey[i]])
                        select left;
            }
            return query;
        }
    }
}
