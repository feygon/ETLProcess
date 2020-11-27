using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalCoreLib
{
    public static class Inheritance
    {
        /// <summary>
        /// Return true if this class was at one point derived from the base type.
        /// </summary>
        /// <param name="derivedType"></param>
        /// <returns></returns>
        public static bool IsDerivedFromBaseType(Type derivedType, Type baseType)
        {
            if (derivedType.BaseType.Name == typeof(object).Name) { return false; }
            if (derivedType.BaseType.Name == baseType.Name) { return true; }
            return IsDerivedFromBaseType(derivedType, baseType);
        }
    }
}
