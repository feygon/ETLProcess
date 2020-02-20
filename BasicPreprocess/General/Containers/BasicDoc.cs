using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace BasicPreprocess.General.Containers
{
    using StringMap = Dictionary<string, string>;

    /// <summary>
    /// An abstract class for wrapping implementation-specific document types in easily accessible expectations of their contents.
    /// <br>See members for details.</br>
    /// </summary>
    public abstract class BasicDoc
    {
        /// <summary>
        /// The name of the column which is the primary key for this class.
        /// </summary>
        public string primaryKey;
        /// <summary>
        /// The names of the columns which are the composite keys for this class, in order of primary, secondary.
        /// </summary>
        public (string primary, string secondary) compositeKey;
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
        ///     unique/redundant primary or composite keyed CSV document classes.
        /// </summary>
        /// <param name="headers">an array of column headers</param>
        /// <param name="primary">String of primary key.</param>
        /// <param name="secondary">String of composite key's secondary column, if any.</param>
        /// <param name="keyIsUniqueIdentifier">Is the primary or composite key a unique identifier?
        /// <br>(Sometimes they're not.)</br></param>
        public BasicDoc(string[] headers, string primary, string secondary = null, bool keyIsUniqueIdentifier = true)
        {
            primaryKey = primary;
            if (secondary != null) { compositeKey = (primaryKey, secondary); }
            this.keyIsUniqueIdentifier = keyIsUniqueIdentifier;
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
        
        private const string MissingValueTag = "NULL";

        /// <summary>
        /// Return empty string if MissingValueTab tripped on dereference.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected static string FilterMissingValue(string s)
    => IsMissingValue(s) ? "" : s;

        /// <summary>
        /// Is a value missing from dereferenceability?
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected static bool IsMissingValue(string s)
            => s.Equals(MissingValueTag, StringComparison.InvariantCultureIgnoreCase);
    }
}
