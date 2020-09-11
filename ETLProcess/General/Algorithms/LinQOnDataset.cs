using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ETLProcess.General.Algorithms
{
    public class LinQOnDataset
    {
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
