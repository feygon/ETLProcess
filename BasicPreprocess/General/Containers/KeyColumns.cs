using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicPreprocess.General.Interfaces;
using BasicPreprocess.General.Containers;

namespace BasicPreprocess.General.Containers
{
    using StringMap = Dictionary<string, string>;

    /// <summary>
    /// A dictionary of keys (and corresponding indices, any), constructible by the desired key type.
    /// </summary>
    /// <typeparam name="TBasicDoc"></typeparam>
    internal sealed class KeyColumns<TBasicDoc> //: Dictionary<_TKeyTypes, TBasicDoc>
        where TBasicDoc : BasicDoc, IDoc<TBasicDoc>, new()
    {
        public _TKeyTypes keysColumnsByType;

        public KeyColumns() : base() { }

        /// <summary>
        /// Constructor of a different types of maps of keys, in a public _TKeyTypes container.
        /// </summary>
        /// <param name="sample">A default constructed sample instance of the TBasicToc generic class.</param>
        /// <param name="docKeyType">The type of key in the source data.</param>
        /// <param name="primaryIndexMap">The map of unique-delineating primary indices, if any.
        ///     <br>ignored if docKeyType is not a redundant primary key.</br>
        /// </param>
        /// <param name="compositeIndexMap">The map of unique-delineating composite indices, if any.
        ///     <br>ignored if docKeyType is not a redundant composite key.</br>
        /// </param>
        public KeyColumns(
                TBasicDoc sample
                //, HeaderSource<List<StringMap>, List<string>> source
                , KeyType docKeyType
                , Auto_Incr_IntMap<string> primaryIndexMap = null
                , Auto_Incr_IntMap<string, string> compositeIndexMap = null
            ) : base()
        {
            // foreach (StringMap doc in source.data)
            // {
            System.Action closure = () =>
            {
                switch (docKeyType) {
                    case KeyType.primaryUnique:
                        keysColumnsByType = new _TKeyTypes(sample.primaryKey);
                        break;

                    case KeyType.primaryRedundant:
                        keysColumnsByType = new _TKeyTypes(sample.primaryKey, primaryIndexMap);
                        break;

                    case KeyType.compositeUnique:
                        keysColumnsByType = new _TKeyTypes(sample.compositeKey);
                        break;

                    case KeyType.compositeRedundant:
                        keysColumnsByType = new _TKeyTypes(sample.compositeKey, compositeIndexMap);
                        break;

                    case KeyType.Error:
                        throw new Exception("Error type populated in document keytyper.");
                }

                //TBasicDoc inner = sample.GetT(doc, source.headers.ToArray());
                //Add(keysColumnsByType, inner); // will just send over the same value each time. No need for foreach.
            }; // end lambda expression
            closure();
            // } // end loop
        } // end method




    } // end clasee
}
