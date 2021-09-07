using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace BasicPreprocess.General.Containers
{
    /// <summary>
    /// A map class of integers which increment starting from 0, mapped on primary keys.
    /// <br>Also available with 2 Generic types.</br>
    /// </summary>
    /// <typeparam name="keyType">Data type of primary member of composite key</typeparam>
    [Serializable]
    public class Auto_Incr_IntMap<keyType> : Dictionary<keyType, int>
    {


        private readonly DateTime tickTime;
        /// <summary>
        /// Default serialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Auto_Incr_IntMap(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            tickTime = DateTime.FromFileTime(info.GetInt64("ticks"));
        }
        
        /// <summary>
        /// required method for serialization interface, inherited from Dictionary.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Construct an empty map. 
        /// </summary>
        public Auto_Incr_IntMap() : base() { }

        /// <summary>
        /// Construct a map en masse, instantiating with a List of key values, 
        ///   being the record-values dereferenced by primary keys (i.e. strings for StringMaps)
        /// </summary>
        /// <param name="primaryKeyColumnValues"></param>
        public Auto_Incr_IntMap(List<keyType> primaryKeyColumnValues) : base()
        {
            foreach (keyType key in primaryKeyColumnValues)
            {
                Add(key);
            }
        }

        /// <summary>
        /// Add a key to the dictionary of generic type of keys.
        /// </summary>
        /// <param name="key"></param>
        public void Add(keyType key)
        {
            base.Add(key, 0);
            Dictionary<string, string> list = new Dictionary<string, string>();
        }
    }

    /*************************************************************************************
        Generic Class Overload
    *************************************************************************************/

    /// <summary>
    /// A map class of integers which increment starting from 0, mapped on composite keys.
    /// <br>Also available with only 1 generic type.</br>
    /// </summary>
    /// <typeparam name="TPrim">Type of primary member of composite key</typeparam>
    /// <typeparam name="TSec">Type of secondary member of composite key</typeparam>
    [Serializable]
    public class Auto_Incr_IntMap<TPrim, TSec> : Dictionary<(TPrim primary, TSec secondary), int>
    {
        private DateTime tickTime;
        /// <summary>
        /// Default serialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Auto_Incr_IntMap(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
            tickTime = DateTime.FromFileTime(info.GetInt64("ticks"));
        }
        /// <summary>
        /// Construct an empty map. 
        /// </summary>
        public Auto_Incr_IntMap() : base() { }

        /// <summary>
        /// required method for serialization interface, inherited from Dictionary.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Add a key to the dictionary of generic type of keys.
        /// </summary>
        /// <param name="key"></param>
        public void Add((TPrim, TSec) key)
        {
            base.Add(key, 0);
            Dictionary<string, string> list = new Dictionary<string, string>();
        }

        /// <summary>
        /// Construct a map en masse, instantiating with a List of keys (i.e. strings for StringMaps)
        /// </summary>
        /// <param name="compositeKeyColumnValues"></param>
        public Auto_Incr_IntMap(List<(TPrim, TSec)> compositeKeyColumnValues) : base()
        {
            foreach ((TPrim, TSec) key in compositeKeyColumnValues)
            {
                Add(key);
            } // end loop
        } // end method
    } // end class
} // end namespace
