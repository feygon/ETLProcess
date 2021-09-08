using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;
using codemap.General.Containers;

namespace BasicPreprocess.General.Containers
{
    using StringMap = Dictionary<string, string>;

    /// <summary>
    /// An abstract class for wrapping implementation-specific document types in easily accessible expectations of their contents.
    /// <br>See members for details.</br>
    /// </summary>
    public abstract class BasicDoc : IBasicDoc
    {
        /// <summary>
        /// Data table where this doc's data row will go.
        /// </summary>
        public DataTable ParentTable { get; set; }
        /// <summary>
        /// Data row where this doc's fields will go.
        /// </summary>
        public DataRow DocRow { get; set; }

        // needed? vvvv

        /// <summary>
        /// The name of the column which is the primary key for this class.
        /// </summary>
        public string primaryKey;
        /// <summary>
        /// The names of the columns which are the composite keys for this class, in order of primary, secondary.
        /// </summary>
        public List<string> compositeKey;
        /// <summary>
        /// Is the primary or composite key a unique identifier, or does it require indexing to be unique?
        /// </summary>
        public bool keyIsUniqueIdentifier;
        /// <summary>
        /// An array of the names of columns in the document data.
        /// </summary>
        public string[] headers;
        
        /// <summary>
        /// Default Constructor -- only here for XMLSerializer. Do not use!
        /// </summary>
        public BasicDoc() { }

        /// <summary>
        /// Constructor: A metadata wrapper for different types of 
        ///     unique/redundant primary keyed document classes.
        /// </summary>
        /// <param name="headers">an array of column headers</param>
        /// <param name="primary">String of primary key.</param>
        /// <param name="keyIsUniqueIdentifier">Is the primary or composite key a unique identifier?
        /// <br>(Sometimes they're not.)</br></param>
        public BasicDoc(string[] headers, string primary, bool keyIsUniqueIdentifier = true)
        {
            primaryKey = primary;
            this.keyIsUniqueIdentifier = keyIsUniqueIdentifier;
            this.headers = headers;
        }

        /// <summary>
        /// Constructor: A metadata wrapper for different types of
        ///     unique/redundant composite keyed document classes.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="composite"></param>
        /// <param name="keyisUniqueIdentifier"></param>
        public BasicDoc(string[] headers, List<string> composite, bool keyisUniqueIdentifier = true)
        {
            compositeKey = composite;
            this.keyIsUniqueIdentifier = keyisUniqueIdentifier;
            this.headers = headers;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="doc">Document to be copied</param>
        public BasicDoc(BasicDoc doc)
        {
            this.compositeKey = doc.compositeKey;
            this.headers = doc.headers;
            this.keyIsUniqueIdentifier = doc.keyIsUniqueIdentifier;
            this.primaryKey = doc.primaryKey;
        }


        /// <summary>
        /// Get decimal value from Stringmap entry on specified column. Called by derived constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="column">The header name of a column where the value in a single record is to be found.</param>
        /// <returns></returns>
        protected static decimal GetDecimalColumn(StringMap data, string column)
        {
            try
            {
                return Parse.DecimalParse(data[column]);
            } catch (Exception inner)
            {
                throw new Exception("Bad column name or unknown exception.", inner);
            }
        }

        /// <summary>
        /// Get date from Stringmap entry on specified column. Called by derived constructor.
        /// </summary>
        /// <param name="data">Stringmap data</param>
        /// <param name="column">specified column of data</param>
        /// <returns></returns>
        protected static Date GetDateColumn(StringMap data, string column)
        {
            try
            {
                return new Date(DateTime.ParseExact(data[column], "yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            catch (Exception inner)
            {
                throw new Exception("Bad column name or unknown exception.", inner);
            }
        }

    }
}
