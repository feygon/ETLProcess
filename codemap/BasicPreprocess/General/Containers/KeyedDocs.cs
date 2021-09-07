using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicPreprocess.General.Containers;
using BasicPreprocess.General.Interfaces;


namespace BasicPreprocess.General.Containers
{
    using StringMap = Dictionary<string, string>;
    
    /// <summary>
    /// Enumerated types of Keys, i.e. primary/composite, unique/indexed
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// A primary key which is unique.
        /// </summary>
        primaryUnique,
        /// <summary>
        /// A composite key which is unique.
        /// </summary>
        compositeUnique,
        /// <summary>
        /// A primary key which requires composition with an index to be unique.
        /// </summary>
        primaryRedundant,
        /// <summary>
        /// A composite key which requires tertiary composition with an index to be unique.
        /// </summary>
        compositeRedundant,
        /// <summary>
        /// Catch-all error enumerator.
        /// </summary>
        Error = -42
    }

    /// <summary>
    /// Container class for lists of documents with various types of keys
    /// </summary>
    /// <typeparam name="TBasicDoc"></typeparam>
    internal sealed class KeyedDocs<TBasicDoc> where TBasicDoc
        : BasicDoc, iDocType_Takes_StringMap<TBasicDoc>, new()
    {
        TBasicDoc sample;
        Auto_Incr_IntMap<string> uniqueInt_byPrimary;
        Auto_Incr_IntMap<string, string> uniqueInt_byComposite;
        private KeyColumns<TBasicDoc> primaryUniqueKeyMap;
        private KeyColumns<TBasicDoc> primaryRedundantKeyMap;
        private KeyColumns<TBasicDoc> compositeUniqueKeyMap;
        private KeyColumns<TBasicDoc> compositeRedundantKeyMap;

        private Dictionary<string, TBasicDoc> primaryKeyedData = new Dictionary<string, TBasicDoc>();
        private Dictionary<ValueTuple<string, string>, TBasicDoc> compositeKeyedData = new Dictionary<(string, string), TBasicDoc>();
        private Dictionary<ValueTuple<string, int>, TBasicDoc> primaryIndexKeyedData = new Dictionary<(string, int), TBasicDoc>();
        private Dictionary<ValueTuple<string, string, int>, TBasicDoc> compositeIndexKeyedData = new Dictionary<(string, string, int), TBasicDoc>();
        KeyType keyType;

        /// <summary>
        /// A class to contain data whether it is primary or composite keyed,
        ///     either redundantly or uniquely.
        ///     <br>TO DO: Could be more general purpose with more explicit interfaces,
        ///         by promising certain generic types for allBasicDocs and headers</br>
        /// </summary>
        /// <param name="source"></param>
        public KeyedDocs(HeaderSource<List<StringMap>, List<string>> source)
        {
            sample = new TBasicDoc(); // is garbage after 6 lines.
            bool composite;
            bool unique;
            try
            {
                if (sample.compositeKey.primary != null)
                {
                    composite = true;
                }
                else { composite = false; }
                unique = sample.keyIsUniqueIdentifier;
            }
            catch
            {
                throw new Exception("Empty data set or unknown exception.");
            }

            if (!composite && unique) { keyType = KeyType.primaryUnique; }
            if (!composite && !unique) { keyType = KeyType.primaryRedundant; }
            if (composite && unique) { keyType = KeyType.compositeUnique; }
            if (composite && !unique) { keyType = KeyType.compositeRedundant; }

            switch (keyType)
            {
                case KeyType.primaryUnique:
                    primaryUniqueKeyMap = new KeyColumns<TBasicDoc>(sample, KeyType.primaryUnique);
                    foreach (StringMap doc in source.data)
                    {
                        System.Action closure = () =>
                        {
                            TBasicDoc inner = sample.GetT(doc, source.headers.ToArray());
                            primaryKeyedData.Add(primaryUniqueKeyMap.keysColumnsByType.primaryKey, inner);
                        };
                    }
                    break;

                case KeyType.primaryRedundant:
                    uniqueInt_byPrimary = new Auto_Incr_IntMap<string>(source.headers);
                    primaryRedundantKeyMap = new KeyColumns<TBasicDoc>(
                        sample
                        , KeyType.primaryRedundant
                        , primaryIndexMap: uniqueInt_byPrimary);
                    primaryIndexKeyedData = new Dictionary<(string, int), TBasicDoc>();

                    foreach (StringMap doc in source.data)
                    {
                        System.Action closure = () =>
                        {
                            // get doc object, add to primary keyed dictionary
                            TBasicDoc inner = sample.GetT(doc, source.headers.ToArray());
                            string primaryKeyColumn = primaryRedundantKeyMap.keysColumnsByType.redundandPrimaryKey.Item1;
                            (string primary, int index) primaryIndexedKey =
                                primaryRedundantKeyMap.keysColumnsByType.GetNextRedundantPrimaryKey(doc[primaryKeyColumn]);

                            primaryIndexKeyedData.Add(primaryIndexedKey, inner);
                            uniqueInt_byPrimary[sample.primaryKey]++;
                        };
                        closure();
                    }
                    break;

                case KeyType.compositeUnique:
                    compositeUniqueKeyMap = new KeyColumns<TBasicDoc>(sample, KeyType.compositeUnique);
                    compositeKeyedData = new Dictionary<(string, string), TBasicDoc>();
                    foreach (StringMap doc in source.data)
                    {
                        System.Action closure = () =>
                        {
                            TBasicDoc inner = sample.GetT(doc, source.headers.ToArray());
                            (string primary, string secondary) compositeKey = (
                                compositeUniqueKeyMap.keysColumnsByType.compositeKey.Item1
                                , compositeUniqueKeyMap.keysColumnsByType.compositeKey.Item2);
                            compositeKeyedData.Add(compositeKey, inner);
                        };
                    }
                    break;

                case KeyType.compositeRedundant:

                    List<(string, string)> keyColumnValues = new List<(string, string)>();


                    uniqueInt_byComposite = new Auto_Incr_IntMap<string, string>(keyColumnValues);
                    compositeRedundantKeyMap = new KeyColumns<TBasicDoc>(
                        sample
                        , KeyType.compositeRedundant
                        , compositeIndexMap: uniqueInt_byComposite);
                    compositeIndexKeyedData = new Dictionary<(string, string, int), TBasicDoc>();

                    foreach (StringMap doc in source.data)
                    {
                        TBasicDoc inner = sample.GetT(doc, source.headers.ToArray());

                        (string, string) compositeKey_isolated = (
                            compositeRedundantKeyMap.keysColumnsByType.redundantCompositeKey.Item1
                            , compositeRedundantKeyMap.keysColumnsByType.redundantCompositeKey.Item2);
                        (string, string, int) compositeIndexedKey =
                            compositeRedundantKeyMap.keysColumnsByType.GetNextRedundantCompositeKey(compositeKey_isolated);

                        compositeIndexKeyedData.Add(compositeIndexedKey, inner);
                    }
                    break;
            }
        }

        /// <summary>
        /// A constructor that takes only unique keys, and uses Linq to generate a selection of these documents.
        /// </summary>
        /// <param name="keyType">An enumeration of which key types is intended to use.</param>
        /// <param name="keyedDocs">The documents to be Selected from.</param>
        /// <param name="columns">A selection of key columns, only one of which is useful at a time.</param>
        /// <returns></returns>
        public KeyedDocs<TBasicDoc> FilterDocs(
            KeyedDocs<TBasicDoc> keyedDocs
            , KeyType keyType
            , (
                string primaryKeyColumns
                , (string, int) primaryIndexedKeyColumns
                , (string, string) compositeKeyColumns
                , (string, string, int)compositeIndexedKeyColumns
            ) columns
            )
        {
            List<StringMap> filteredRecords = new List<StringMap>();
            HeaderSource<List<StringMap>, List<String>> filteredData;
            KeyedDocs<TBasicDoc> retDocs;

            // only add filtered records to this.
            switch (keyType)
            {
                case KeyType.primaryUnique:
                    string primaryKeyColumns = columns.primaryKeyColumns;

                    /*
                     * from data in columns named in primary keyed columns
                     * where keys match those in primary
                     * 
                     * 
                     * */

                    foreach (KeyValuePair<string, TBasicDoc> record in keyedDocs.primaryKeyedData)
                    {
                        
                    }
                    throw new NotImplementedException("filteredData, switch case");
                    break;
                case KeyType.primaryRedundant:
                    (string, int) primaryIndexedKeyColumns = columns.primaryIndexedKeyColumns;
                    throw new NotImplementedException("filteredData, switch case");
                    break;
                case KeyType.compositeUnique:
                    (string, string) compositeKeyColumns = columns.compositeKeyColumns;
                    throw new NotImplementedException("filteredData, switch case");
                    break;
                case KeyType.compositeRedundant:
                    (string, string, int) compositeIndexedKeyColumns = columns.compositeIndexedKeyColumns;
                    throw new NotImplementedException("filteredData, switch case");
                    break;
                case KeyType.Error:
                    throw new Exception("Error Keytype thrown.");
                    break;
            }

            filteredData = new HeaderSource<List<StringMap>, List<string>>();
            retDocs = new KeyedDocs<TBasicDoc>(filteredData);
            return retDocs;

        }

        /// <summary>
        /// Derefrence a specific document with a primary key and an index, if any.
        /// </summary>
        /// <param name="primaryKey">The primary key</param>
        /// <param name="index">The numeric index, if any.</param>
        /// <returns></returns>
        public TBasicDoc this[string primaryKey, int index = -42] {
            get {
                TBasicDoc ret;
                if (keyType == KeyType.primaryUnique)
                {
                    primaryKeyedData.TryGetValue(primaryKey, out ret);
                } 
                else if (keyType == KeyType.primaryRedundant && index >= 0)
                {
                    primaryIndexKeyedData.TryGetValue((primaryKey, index), out ret);
                } 
                else
                {
                    // if primary keys references a primary redundant key, possibility that it returns a List<TBasicDoc>
                    throw new NullReferenceException("Wrong key type. Not a primary keyed data set.");
                }
                return ret;
            }
        }
        /// <summary>
        /// Dereference a primary indexed data set to a single index of similarly composite-keyed records.
        /// </summary>
        /// <param name="primaryIndexedKey">The redundant primary key without its index.</param>
        /// <param name="keyType">What type of key does this document take?</param>
        /// <returns></returns>
        public Dictionary<int, TBasicDoc> this[string primaryIndexedKey, KeyType keyType]
        {
            get {
                Dictionary<int, TBasicDoc> ret = new Dictionary<int, TBasicDoc>();
                if (keyType == KeyType.primaryRedundant
                    && this.keyType == KeyType.primaryRedundant)
                {
                    Dictionary<int, string> indexedRecords = new Dictionary<int, string>();
                    foreach(KeyValuePair<(string primary, int index), TBasicDoc> kvp in primaryIndexKeyedData)
                    {
                        if (kvp.Key.primary == primaryIndexedKey)
                        {
                            ret.Add(kvp.Key.index, kvp.Value);
                        }
                    }
                    return ret;
                } else
                {
                    throw new NullReferenceException("Wrong key type. Not a primary indexed data set.");
                }
            }
        }

        /// <summary>
        /// Dereferece a specific document on its composite key and index, if any.
        /// </summary>
        /// <param name="compositeKey">The composite key of two strings</param>
        /// <param name="index">The numeric index, if any</param>
        /// <returns></returns>
        public TBasicDoc this[
            (string primary, string secondary) compositeKey
            , int index = -42]
        {
            get {
                TBasicDoc ret = null;
                if (keyType == KeyType.compositeUnique)
                {
                    compositeKeyedData.TryGetValue(compositeKey, out ret);
                } else if ( keyType == KeyType.compositeRedundant && index >= 0)
                {
                    compositeIndexKeyedData.TryGetValue(
                        (compositeKey.primary, compositeKey.secondary, index)
                        , out ret);
                } else
                {
                    throw new NullReferenceException("Wrong key type. Not a primary indexed data set.");
                }
                return ret;
            }
        }

        /// <summary>
        /// Dereference a composite indexed data set to a single index of similarly composite-keyed records.
        /// </summary>
        /// <param name="compositeIndexedKey">The shared composite key</param>
        /// <param name="keyType">What kind of key does this document take?</param>
        /// <returns></returns>
        public Dictionary<int, TBasicDoc> this[(string, string) compositeIndexedKey, KeyType keyType]
        {
            get
            {
                Dictionary<int, TBasicDoc> ret = new Dictionary<int, TBasicDoc>();
                if (keyType == KeyType.compositeRedundant 
                    && this.keyType == KeyType.compositeRedundant)
                {
                    foreach (KeyValuePair<(string primary, string secondary, int index), TBasicDoc> kvp in compositeIndexKeyedData)
                    {
                        if (kvp.Key.primary == compositeIndexedKey.Item1 
                            && kvp.Key.secondary == compositeIndexedKey.Item2)
                        {
                            ret.Add(kvp.Key.index, kvp.Value);
                        }
                    }
                    return ret;
                }
                else
                {
                    throw new NullReferenceException("Wrong key type. Not a primary indexed data set.");
                }
            }
        }
    }
}
