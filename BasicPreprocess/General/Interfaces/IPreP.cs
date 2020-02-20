using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPreprocess.General.Interfaces
{
    /// <summary>
    /// Promise that a General preprocess 
    /// </summary>
    public interface IPreP
    {
        /// <summary>
        /// Promise that a preprocess class will provide files.
        /// </summary>
        public string[] files { get; set; }
        /// <summary>
        /// Promise that a preprocess class will provide debug logging.
        /// </summary>
        public Log Debug { get; set; }
        /// <summary>
        /// Promise that a preprocess class will set its own file path.
        /// </summary>
        /// <param name="filename"></param>
        public void SetDebug(string filename);
        /// <summary>
        /// Promise that a preprocess class will be able to run a specific class's file check function.
        /// </summary>
        /// <param name="checkFiles">A function defined in the implementation which checks files 
        /// according to the specific process's requirements.</param>
        public bool Check_Files(DelRet<bool, string> checkFiles);
        /// <summary>
        /// Promise that an unzipping preprocess class will set the temporary location where files are unzipped.
        /// </summary>
        /// <param name="filename"></param>
        void SetTempDir(string filename);
    }
}
