using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicPreprocess.General.Interfaces;

namespace BasicPreprocess.General.Containers
{
    /// <summary>
    /// A class to track the names of columns used as unique identifiers in tabular data, 
    /// and if necessary, hold a column of auto-incrementing indexes which may be dereferenced 
    /// on instances of their accompanying keys.
    /// <para>Probably not used outside of other General.Containers->KeyColumns internal sealed class.</para>
    /// </summary>
    public class _TKeyTypes<TBasicRecord> where TBasicRecord: BasicRecord, IRecord<TBasicRecord>, new()
    {
        /// <summary>
        /// Primary or composite key
        /// </summary>
        public List<string> recordKey;
        /// <summary>
        /// Index for primary or composite key, if it is redundant.
        /// </summary>
        public Auto_Incr_IntMap<TBasicRecord> redundantKeyIndex;

        /// <summary>
        /// Build a public tuple with a value in the composite key.
        /// </summary>
        /// <param name="recordKey">A key with two strings which together are a unique combination of strings in their columns.</param>
        public _TKeyTypes(List<string> recordKey)
        {
            //this.primaryKey = null;
            //this.redundantPrimaryKeyIndex = null;
            this.recordKey = recordKey;
            this.redundantKeyIndex = null;
        }

        /// <summary>
        /// Build a public tuple with a value in the compositeRedundant key.
        /// </summary>
        /// <param name="compositeKey">A key with two strings which together are a combination of strings
        ///     in their columns which are redundant with only a few others.</param>
        /// <param name="compositeIndexMap">A map in index ints, mapped on the composite key.</param>
        public _TKeyTypes(List<string> compositeKey
            , Auto_Incr_IntMap<TBasicRecord> compositeIndexMap)
        {
            //this.primaryKey = null;
            //this.redundantPrimaryKeyIndex = null;
            this.recordKey = compositeKey;
            this.redundantKeyIndex = compositeIndexMap;
        }
    }
}
