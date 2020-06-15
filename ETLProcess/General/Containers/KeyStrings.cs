using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProcess.General.Containers
{
    /// <summary>
    /// A class of string lists with a comparator.
    /// </summary>
    public class KeyStrings : List<string>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public KeyStrings() : base() { }
        
        /// <summary>
        /// Deep equality comparer
        /// </summary>
        /// <param name="left">The list of strings to compare.</param>
        /// <param name="right">The list of strings to compare this to.</param>
        /// <returns></returns>
        public static bool operator ==(KeyStrings left, KeyStrings right)
        {
            bool same = false;
            if (left.Count != right.Count) { return same; }
            for (int i = 0; i < left.Count; i++)
            {
                if (left[i] != right[i]) { return same; }
            }
            same = true;
            return same;
        }

        /// <summary>
        /// Deep inequality comparer
        /// </summary>
        /// <param name="left">The list of strings to compare.</param>
        /// <param name="right">The list of strings to compare this to.</param>
        /// <returns></returns>
        public static bool operator !=(KeyStrings left, KeyStrings right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Deep equality comparer for string array.
        /// </summary>
        /// <param name="left">The list of strings to compare.</param>
        /// <param name="right">The array of strings to compare this to.</param>
        /// <returns></returns>
        public static bool operator ==(KeyStrings left, string[] right)
        {
            return (left == (right.ToList()) as KeyStrings);
        }

        /// <summary>
        /// Deep inequality comparer for string array.
        /// </summary>
        /// <param name="left">The list of strings to compare.</param>
        /// <param name="right">The array of strings to compare this to.</param>
        /// <returns></returns>
        public static bool operator !=(KeyStrings left, string[] right)
        {
            return !(left == (right.ToList()) as KeyStrings);
        }

        /// <summary>
        /// Equals override, same as base.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// GetHashCode, same as base.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
