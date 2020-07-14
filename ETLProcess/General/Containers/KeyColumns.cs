using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETLProcess.General.Interfaces;
using ETLProcess.General.Containers;

namespace ETLProcess.General.Containers
{
    using StringMap = Dictionary<string, string>;

    /// <summary>
    /// A dictionary of keys (and corresponding indices, any), constructible by the desired key type.
    /// </summary>
    /// <typeparam name="TBasicRecord"></typeparam>
    internal sealed class KeyColumns<TBasicRecord> //: Dictionary<_TKeyTypes, TBasicDoc>
        where TBasicRecord : BasicRecord, IRecord<TBasicRecord>, new()
    {
        public _TKeyTypes<TBasicRecord> keysColumnsByType;

        public KeyColumns() {
            
        }

        /// <summary>
        /// Constructor of a different types of maps of keys, in a public _TKeyTypes container.
        /// </summary>
        /// <param name="sample">A default constructed sample instance of the TBasicToc generic class.</param>
        /// <param name="indexMap">The map of unique-delineating composite indices, if any.
        ///     <br>ignored if docKeyType is not a redundant composite key.</br>
        /// </param>
        public KeyColumns(
                TBasicRecord sample
                //, HeaderSource<List<StringMap>, List<string>> source
                //, Auto_Incr_IntMap<string> primaryIndexMap = null
                , Auto_Incr_IntMap<TBasicRecord> indexMap = null
            )
        {
            // foreach (StringMap doc in source.data)
            // {
            System.Action closure = () =>
            {
                /* deprecated
                //switch (docKeyType) {
                //case KeyType.primaryUnique:
                //    keysColumnsByType = new _TKeyTypes(sample.primaryKey);
                //    break;

                //case KeyType.primaryRedundant:
                //    keysColumnsByType = new _TKeyTypes(sample.primaryKey, primaryIndexMap);
                //    break;
                */
                if (sample.keyIsUniqueIdentifier)
                {
                    keysColumnsByType = new _TKeyTypes<TBasicRecord>(sample.recordKey);
                } else {
                    keysColumnsByType = new _TKeyTypes<TBasicRecord>(sample.recordKey, indexMap);
                }
                    //case KeyType.Error:
                    //    throw new Exception("Error type populated in document keytyper.");
                //}

                //TBasicDoc inner = sample.GetT(doc, source.headers.ToArray());
                //Add(keysColumnsByType, inner); // will just send over the same value each time. No need for foreach.
            }; // end lambda expression
            closure();
            // } // end loop
        } // end method
    } // end class
}
