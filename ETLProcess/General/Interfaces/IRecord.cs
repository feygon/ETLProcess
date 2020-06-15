using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETLProcess.General.Containers;

namespace ETLProcess.General.Interfaces
{
    /// <summary>
    /// An interface to promise that each document based upon a basic doc will implement (a) certain method(s).
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    public interface IRecord<TRecord> where TRecord : BasicRecord
    {

        /// <summary>
        /// An array of the names of columns in the document data.
        /// </summary>
        public List<string> headers { get; }

        /// <summary>
        /// A dictionary of the types that each column will have in the final document, by header.
        /// </summary>
        public Dictionary<string, Type> columnTypes { get; }
        /// <summary>
        /// Called by sample.
        /// <br>A function is required which takes a delegate of the 'getDocStringMap' configuration.</br>
        /// <br>This will promise that a constructor is called which uses a StringMap as an argument, and may use an array of strings.</br>
        /// <code>(See above comment block for sample code)</code>
        /// </summary>
        /// <param name="stringMap">A sample record</param>
        /// <param name="headers">The headers for the given document</param>
        /// <returns></returns>
        public TRecord Record(
            StringMap stringMap
            , List<string> headers);


        // Code guidance for constructors:
        /*
            //Default constructor example:
            public TRecord() : 
                base(List<string> headers // all column names
                , List<string> keyHeaders // composite key column names
                , StringMap data);
        */

        /*
            //Copy constructor example:
            public TRecord(TRecord doc)
            : base(
                 headers: doc.headers.ToArray().ToList()
                 , keyHeaders: new string[] { "Key1", "Key2", ... }.ToList() // composite key column names
                 , data: null // directly copy values in constructor body.
                 , keyIsUniqueIdentifier: doc.keyIsUniqueIdentifier
         */

        /*
            //Constructor that takes a StringMap and headers (used in concert with Record method above, which is called by accessing sample).
            public StatementRecords(StringMap data, List<string> headers) 
            : base(
                  headers: headers
                  , keyHeaders: new string[] { "Key1", "Key2", ... }.ToList() // composite key column names
                  , data: data
                  , keyIsUniqueIdentifier: true)
        */
    }
}