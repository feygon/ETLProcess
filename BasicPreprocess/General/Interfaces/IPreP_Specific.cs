using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPreprocess.General.Interfaces
{


    /// <summary>
    /// Interface to promise the structure of a preprocess which receives files from a ZipFile.
    /// </summary>
    public interface IPreP_Specific<TIPreP> where TIPreP : IPreP
    {
        /// <summary>
        /// Promise that if there are any implementation-specific preprocessor definitions,
        ///  the class gets them and put them into effect.
        /// </summary>
        public void GetImplementation(string imp_Arg = null);

        /// <summary>
        /// Promise that a specific implementation object will check files for 
        ///  proper naming and integrity requirements.
        /// </summary>
        public DelRet<bool, string> checkFiles_Delegate { get; }

        /// <summary>
        /// Promise that specific pre-processes will provide a way to identify document types.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public DocType IdentifyDoc(string filename);
    }
}
