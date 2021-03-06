using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETLProcessFactory.Containers;
using ETLProcessFactory;
using ETLProcessFactory.GP;

namespace ETLProcessFactory.Interfaces
{
    /// <summary>
    /// An interface to promise that each document based upon a basic doc will implement (a) certain method(s).
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    public interface IRecord_Uses_ImportRows<TRecord> where TRecord : BasicRecord<TRecord>, IRecord<TRecord>, new()
    {
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