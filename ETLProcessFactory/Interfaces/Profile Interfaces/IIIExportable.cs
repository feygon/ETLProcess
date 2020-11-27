using ETLProcessFactory.Containers.Dictionaries;
using UniversalCoreLib;

namespace ETLProcessFactory.Interfaces.Profile_Interfaces
{
    /// <summary>
    /// Promise that a class will implement output profile methods.
    /// </summary>
    public interface IIIExportable
    {
        /// <summary>
        /// Promise that a class implementing this profile will check its output for validity.
        /// </summary>
        /// <param name="outputs"></param>
        /// <returns></returns>
        public bool Check_Output(DelRet<bool, string[]> outputs);
    }
}