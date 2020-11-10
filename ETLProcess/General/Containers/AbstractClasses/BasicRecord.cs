using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.CodeDom;
using System.ComponentModel;
using System.Data;
using System.Reflection;

using ETLProcess.General.Containers.Members;
using ETLProcess.General.Interfaces;
using ETLProcess.General.Containers.AbstractClasses;
using ETLProcess.Specific.Boilerplate;
using ETLProcess.General.IO;

using SampleColumnTypes = System.Collections.Generic.Dictionary<string, System.Type>;

namespace ETLProcess.General.Containers
{
    /// <summary>
    /// An abstract class for wrapping implementation-specific document types in easily accessible expectations of their contents.
    /// <br>See members for details.</br>
    /// </summary>
    public abstract class BasicRecord<T> : StringMap where T : BasicRecord<T>, new()
    {
        /// <summary>
        /// Singleton sample accessor for child class. Child class should have constraint: "BasicRecord-ChildClass",
        ///     where Childclass is in generic brackets.
        /// </summary>
        public static T Sample { get; private set; } = null;

        /// <summary>
        /// Initializer for Static Sample.
        /// </summary>
        public static void InitSample() { Sample = new T(); }

        /// <summary>
        /// The values in the columns which hold composite keys for this class.
        /// </summary>
        public KeyStrings recordKey;
        /// <summary>
        /// Is the primary or composite key a unique identifier, or does it require indexing to be unique?
        /// </summary>
        public bool keyIsUniqueIdentifier;
        /// <summary>
        /// The unique index of this primary or composite key.
        /// </summary>
        public int uniqueIndex;
        /// <summary>
        /// An action to get a child record type.
        /// </summary>
        public DelRet<Type> action_GetRecordType;

        /// <summary>
        /// Default Constructor -- only here for XMLSerializer. Do not use!
        /// </summary>
        public BasicRecord() : base(){}

        /// <summary>
        /// Accessor for child class headers string list.
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetHeaders();

        /// <summary>
        /// Return GetType for child.
        /// </summary>
        /// <returns></returns>
        public abstract Type GetChildType();

        /// <summary>
        /// Constructor: A metadata wrapper for different types of 
        ///     unique/redundant primary or composite keyed CSV document classes.
        /// </summary>
        /// <param name="data">Data from which to derive a key, if any.</param>
        /// <param name="sampleColumnTypes">A dictionary of column types in the derived type of Record</param>
        /// <param name="keyIsUniqueIdentifier">Is the primary or composite key a unique identifier?
        /// <br>(Sometimes they're not.)</br></param>
        public BasicRecord(
            StringMap data
            , TableHeaders sampleColumnTypes
            , bool keyIsUniqueIdentifier = true) : base()
        {
            try
            {
                foreach (KeyValuePair<string, string> record in data)
                {
                    Add(record.Key, record.Value);
                }

                recordKey = new KeyStrings();

                // TO DO: Decouple from clientETLProcess (should be providable from FileDataRecords).
                foreach (KeyValuePair<string, (Type colType, bool isKey)> headerData
                    in sampleColumnTypes.Where((x)=> x.Value.isKey == true))
                {
                    bool success = data.TryGetValue(headerData.Key, out string dataValue);
                    if (success)
                    {
                        recordKey.Add((dataValue ??= data[headerData.Key], headerData.Key));
                    }
                    else
                    {
                        Log.WriteException("Bad key assignent or unknown failure in TryGetValue.");
                    }
                }
            }catch (WarningException err)
            {
                Log.Write("ETLProcess threw Warning: " + err.ToString() + "at BasicRecord constructor.");
            }
            this.keyIsUniqueIdentifier = keyIsUniqueIdentifier;
            //this.headers = keyHeaders;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="record">Object to be copied from</param>
        public BasicRecord(BasicRecord<T> record)
        {
            this.recordKey = record.recordKey.ToArray().ToList() as KeyStrings; // force copy
            this.keyIsUniqueIdentifier = record.keyIsUniqueIdentifier;
            foreach (KeyValuePair<string, string> cell in record)
            {
                Add(cell.Key, cell.Value);
            }
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