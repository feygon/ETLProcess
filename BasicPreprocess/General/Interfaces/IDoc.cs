using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicPreprocess.General.Containers;

namespace BasicPreprocess.General.Interfaces
{
    /// <summary>
    /// An interface to promise that each document based upon a basic doc will implement (a) certain method(s).
    /// </summary>
    /// <typeparam name="TDoc"></typeparam>
    public interface IDoc<TDoc> where TDoc : BasicDoc
    {

        /* Example Code
        public DocM690_MemberRecord GetT( StringMap stringMap, string[] headers)
        {
            return new DocM690_MemberRecord(headers, stringMap);
        }
        */

        

        /// <summary>
        /// Called by sample.
        /// <br>A function is required which takes a delegate of the 'getDocStringMap' configuration.</br>
        /// <br>This will promise that a constructor is called which uses a StringMap as an argument, and may use an array of strings.</br>
        /// <code>(See above comment block for sample code)</code>
        /// </summary>
        /// <param name="stringMap">A sample record</param>
        /// <param name="headers">The headers for the given document</param>
        /// <returns></returns>
        public TDoc GetT(
            Dictionary<string, string> stringMap
            , string[] headers);
    }
}