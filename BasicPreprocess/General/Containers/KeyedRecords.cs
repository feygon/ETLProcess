using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using BasicPreprocess.General.Containers;
using BasicPreprocess.General.Interfaces;


namespace BasicPreprocess.General.Containers
{
    using StringMap = Dictionary<string, string>;
    
    /// <summary>
    /// Container class for lists of documents with various types of keys
    /// </summary>
    /// <typeparam name="TBasicRecord"></typeparam>
    internal sealed class KeyedRecords<TBasicRecord> where TBasicRecord
        : BasicRecord, IRecord<TBasicRecord>, new()
    {
        TBasicRecord sample;
        public bool unique;

        /// <summary>
        /// Uniquely keyed data dereferenceable with TryGetValue, with a single record instance out.
        /// </summary>
        private Dictionary<List<string>, TBasicRecord> keyedData =
            new Dictionary<List<string>, TBasicRecord>();
        /// <summary>
        /// Uniquely keyed data dereferenceable with TryGetValue, with a list of records out.
        /// </summary>
        private Dictionary<List<string>, List<TBasicRecord>> indexKeyedData =
            new Dictionary<List<string>, List<TBasicRecord>>();

        /// <summary>
        /// A class to contain data whether it is primary or composite keyed,
        ///     either redundantly or uniquely.
        ///     <br>TO DO: Could be more general purpose with more explicit interfaces,
        ///         by promising certain generic types for allBasicDocs and headers</br>
        /// </summary>
        /// <param name="source">Each Stringmap in the List of Stringmaps is a line of strings, with a string for a column. 
        /// <br>The List of strings after this is the headers.</br></param>
        public KeyedRecords(HeaderSource<List<StringMap>, List<string>> source)
        {
            sample = new TBasicRecord();
            //bool composite;
            try
            {
                unique = sample.keyIsUniqueIdentifier;
            } catch {
                throw new Exception("Empty data set or unknown exception.");
            }
            { 
            /*
            if (sample.keyIsUniqueIdentifier) {
                keyedData = new Dictionary<List<string>, TBasicRecord>();
            }
            else {
                indexKeyedData = new Dictionary<List<string>, List<TBasicRecord>>();// new Dictionary<(List<string>, int), TBasicRecord>();
            }
            */
            } // Deprecated code.

            // Build a Dictionary of TBasicRecords from a selection of instantiated records,
            //  on the key for each record.
            if (sample.keyIsUniqueIdentifier) {
                var keyed = source.data.Select((line)=>
                {
                    TBasicRecord record = sample.GetRecord(line, source.headers);
                    return new KeyValuePair<List<string>, TBasicRecord>(record.recordKey, record);
                });
                try
                {
                    keyedData = keyed.ToDictionary(pair => pair.Key, pair => pair.Value);
                } catch (System.Exception err) { 
                    throw new Exception("Badly formed IEnumerable or unknown exception.", err); 
                }
            } 
            // Build a Dictionary of Indexed TBasicRecord lists, from a seleciton of instantiated records,
            //  first checking whether a key exists in the dictionary, then adding it.
            else {
                var unindexed = source.data.Select((line) =>
                {
                    TBasicRecord record = sample.GetRecord(line, source.headers);
                    return new KeyValuePair<List<string>, TBasicRecord>();
                });
                Dictionary<List<string>, List<TBasicRecord>> indexed = new Dictionary<List<string>, List<TBasicRecord>>();
                foreach (KeyValuePair<List<string>, TBasicRecord> uirec in unindexed)
                {
                    List<TBasicRecord> list;
                    if (indexed.TryGetValue(uirec.Key, out list) == true) { 
                        list.Add(uirec.Value); 
                    } else {
                        List<TBasicRecord> val = new TBasicRecord[] { uirec.Value }.ToList();
                        indexed.Add(uirec.Key, val);
                    }
                }
                try
                {
                    indexKeyedData = indexed.ToDictionary(pair => pair.Key, pair => pair.Value);
                }
                catch (Exception err)
                {
                    throw new Exception("Badly formed IEnumerable, bad tryGetValue, or unknown exception.", err);
                }
            }
            {/*
                //            foreach (StringMap recordData in source.data)
                //            {
                //                System.Action closure = () =>
                //                {
                //                    TBasicRecord record = sample.GetRecord(recordData, source.headers);
                //                    try
                //                    {
                //                        if (sample.keyIsUniqueIdentifier)
                //                        {
                //                            keyedData.Add(sample.recordKey, record);
                //                        } else
                //                        {
                //                            // TO DO: Write new method for this in the appropriate class.
                ////                            int newIndex = indexKeyedData.GetNewIndex(sample.recordKey);
                //                            indexKeyedData[sample.recordKey].Add(record);//.Add((sample.recordKey, newindex), record);
                //                        }
                //                    }
                //                    catch (Exception err)
                //                    {
                //                        throw new Exception(String.Format(
                //                            "Error dereferencing on composite key {0} or adding key to primaryKeyedData has thrown innerException: "
                //                            , keyMap.keysColumnsByType.recordKey), err);
                //                    }
                //                };
                //                closure();
                //            }
            */} // Deprecated code.
        }

        /// <summary>
        /// A constructor that takes only unique keys, and uses Linq to generate a selection of these documents.
        /// </summary>
        /// <param name="keyedRecords">The documents to be Selected from.</param>
        /// <param name="columns">A selection of key columns, only one of which is useful at a time.</param>
        /// <returns></returns>
        public KeyedRecords<TBasicRecord> FilterRecords(
            KeyedRecords<TBasicRecord> keyedRecords
            , ( List<string> keyColumns, 
                (List<string>, int)indexedKeyColumns) columns
            )
        {
            List<StringMap> filteredRecords = new List<StringMap>();
            HeaderSource<List<StringMap>, List<String>> filteredData;
            KeyedRecords<TBasicRecord> retDocs;

            if (this.unique) {
                List<string> keyColumns = columns.keyColumns;

                //clientBusinessRules.FilterBy_xxx?

                // Not implemented here.
            } else {
                (List<string>, int) indexedKeyColumns = columns.indexedKeyColumns;
                //clientBusinessRules.FilterBy_xxx?
                
                // Not implemented here.
            }

            filteredData = new HeaderSource<List<StringMap>, List<string>>();
            retDocs = new KeyedRecords<TBasicRecord>(filteredData);
            throw new NotImplementedException("Filtration not implemented in if/then clauses.");
            return retDocs;

        }

        /// <summary>
        /// Dereference a single record on its unique composite key.
        /// </summary>
        /// <param name="recordKey">The unique key</param>
        /// <param name="ret">The return object.</param>
        /// <returns></returns>
        public bool TryGetValue (
            List<string> recordKey, out TBasicRecord ret)
        {
            if (!this.unique)
            {
                throw new NullReferenceException("Invalid access. Not a unique data instance.");
            }
            keyedData.TryGetValue(recordKey, out ret);
            return true;
        }

        /// <summary>
        /// Dereference a list of records on their shared composite key.
        /// </summary>
        /// <param name="recordKey">The redundant key</param>
        /// <param name="ret">The return list.</param>
        /// <returns></returns>
        public bool TryGetValue (List<string> recordKey, out List<TBasicRecord> ret)
        {
            if (this.unique)
            {
                throw new NullReferenceException("Invalid access. Not an indexed data set.");
            }
            indexKeyedData.TryGetValue(recordKey, out ret);
            return true;
        }
    } // end class.
} // end namespace.
