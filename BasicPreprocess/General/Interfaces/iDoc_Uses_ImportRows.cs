using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicPreprocess.General.Containers;

namespace BasicPreprocess.General.Interfaces
{
    using StringMap = Dictionary<string, string>;
    /// <summary>
    /// An interface to promise that each document based upon a basic doc will implement (a) certain method(s).
    /// </summary>
    /// <typeparam name="TDoc"></typeparam>
    public interface IDoc_Uses_ImportRows<TDoc> where TDoc : BasicDoc, IDoc<TDoc>
    {
        /* Example Code
        public DocM690_MemberRecord GetT( StringMap stringMap, string[] headers)
        {
            return new DocM690_MemberRecord(headers, stringMap);
        }
        */

        /// <summary>
        /// A promise that a default constructed instance of a class will be able to construct
        ///     the needed data structure to build a KeyedDocs container for multiple records,
        ///     using the lines returned from 'ImportRows'.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public HeaderSource<List<StringMap>, List<string>> ParseRows(string[] lines);
    }
}