using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPreprocess.General
{
    /// <summary>
    /// a delegate that takes any type and doesn't return anything.
    /// </summary>
    /// <typeparam name="T1">The type (including tuples)</typeparam>
    /// <param name="t1">The type instance to be passed.</param>
    public delegate void DelVoid<T1>(T1 t1);

    /// <summary>
    /// A delegate that takes any type and returns the other type.
    /// </summary>
    /// <typeparam name="T0">The return type.</typeparam>
    /// <typeparam name="T1">The passed type.</typeparam>
    /// <param name="t1">The passed instance.</param>
    /// <returns>Returns an instance of the return type.</returns>
    public delegate T0 DelRet<T0, T1>(T1[] t1);
}
