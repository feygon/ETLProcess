using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETLProcessFactory.Containers;
using ETLProcessFactory.Containers.AbstractClasses;
using ETLProcessFactory.GP;
using SampleColumnTypes = System.Collections.Generic.Dictionary<string, System.Type>;

namespace ETLProcessFactory.Interfaces
{
    /// <summary>
    /// An interface to promise that each document based upon a basic doc will implement (a) certain method(s).
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    public interface IRecord<TRecord> where TRecord : BasicRecord<TRecord>, new()
    {
        /// <summary>
        /// An array of the names of columns in the document data.
        /// </summary>
        public List<string> Headers { get; }

        /// <summary>
        /// A dictionary of the types that each column will have in the final document, by header.
        /// </summary>
        public TableHeaders ColumnTypes { get; }
        /// <summary>
        /// Called by sample.
        /// <br>A function is required which takes a delegate of the 'getDocStringMap' configuration.</br>
        /// <br>This will promise that a constructor is called which uses a StringMap as an argument, and may use an array of strings.</br>
        /// <code>(See above comment block for sample code)</code>
        /// </summary>
        /// <param name="stringMap">A sample record
        ///     <para>Explicit cast to base type of type argument may be required.</para></param>
        /// <param name="sampleColumnTypes">A dictionary of column types by header name in the interfaced type of Record</param>
        /// <param name="headers">The headers for the given document</param>
        /// <returns></returns>
        public TRecord Record(
            StringMap stringMap
            , TableHeaders sampleColumnTypes
            , List<string> headers);

        // Code guidance for constructors:
        // TO DO: write this commentary.
    }
}