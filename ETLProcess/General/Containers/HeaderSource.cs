using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ETLProcess.General.Containers
{

    /// <summary>
    /// Wrapper class for a List of document records and their headers.
    /// </summary>
    /// <typeparam name="StringMapList">List of records</typeparam>
    /// <typeparam name="StringList">A list of record headers for data columns.</typeparam>
    public struct HeaderSource<StringMapList, StringList>
        where StringMapList : List<StringMap>
        where StringList : List<string>
    {
        /// <summary>
        /// List of column header strings
        /// </summary>
        public List<string> headers;
        /// <summary>
        /// The data records
        /// </summary>
        public List<StringMap> data;

        /// <summary>
        /// Constructor, takes Data lists and headers.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="headerStrings"></param>
        public HeaderSource(StringMapList data, string[] headerStrings)
        {
            this.data = data;
            this.headers = new List<string>(headerStrings.ToList());
        }
    }
}
