using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using ETLProcess.General.Containers;
using System.Runtime.CompilerServices;

namespace ETLProcess.General.Containers
{
    /// <summary>
    /// A map class of integers which increment starting from 0, mapped on primary keys.
    /// <br>Also available with 2 Generic types.</br>
    /// </summary>
    /// <typeparam name="TRecord">Record type of primary member of composite key<br>
    /// By extension, can be used for BasicDetails too.</br></typeparam>
    [Serializable]
    public class Auto_Incr_IntMap<TRecord> : Dictionary<int, TRecord> where TRecord : BasicRecord
    {
        private readonly DateTime tickTime;
        /// <summary>
        /// Add direct public access to index 0 as a '.record'. attribute.
        /// </summary>
        public TRecord record
        {
            get {
                TRecord temp;
                bool success = TryGetValue(0, out temp);
                if (success)
                {
                    return temp;
                }
                else throw new AccessViolationException("Empty set accessed.");
            }
            set {
                TRecord temp;
                bool success = TryGetValue(0, out temp);
                if (success)
                {
                    temp = value;
                } else throw new AccessViolationException("Empty set mutated.");
            }
        }

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
        public Auto_Incr_IntMap(List<TRecord> primaryKeyColumnValues) : base()
        {
            foreach (TRecord key in primaryKeyColumnValues)
            {
                Add(key);
            }
        }

        /// <summary>
        /// Add a key to the dictionary of generic type of keys.
        /// </summary>
        /// <param name="record">The record to be added to this map.</param>
        public void Add(TRecord record)
        {
            base.Add(Count, record);
        }

        /// <summary>
        ///  Remove record from base set and decrement all higher indices.
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index) 
        {
            TRecord i;
            bool success = TryGetValue(index, out i);
            if (!success) { throw new AccessViolationException("A record does not exist at this index."); }

            base.Remove(index);
            foreach (int ind in this.Keys)
            {
                TRecord temp;
                if (ind > index && TryGetValue(ind, out temp) == true) {
                    base.Add(ind - 1, this[ind]);
                    base.Remove(ind);
                }
            }
        }

        public void Remove(TRecord record)
        {
            // TO DO: Implement
            throw new NotImplementedException("Remove by TRecord not implemented.");
        }

        /// <summary>
        /// Get next unused index.
        /// </summary>
        /// <returns></returns>
        public int GetNewIndex()
        {
            return this.Count;
        }
    } // end class
} // end namespace
