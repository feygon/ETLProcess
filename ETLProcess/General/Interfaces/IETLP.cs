using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProcess.General.Interfaces
{
    /// <summary>
    /// Promise that a General ETLProcess 
    /// </summary>
    public interface IETLP
    {
        /// <summary>
        /// Promise that an ETLProcess class will provide files.
        /// </summary>
        public string[] files { get; set; }
        /// <summary>
        /// Promise that an ETLProcess class will provide debug logging.
        /// </summary>
        public Log Debug { get; set; }
        /// <summary>
        /// Promise that an ETLProcess class will set its own file path for the log.
        /// Set the location of the Debug Log, for use in the constructor.
        /// </summary>
        /// <param name="filename"></param>
        public void InitLog(string filename);
        /// <summary>
        /// Promise that an ETLProcess class will be able to run a specific class's file check function.
        /// </summary>
        /// <param name="checkFiles">A function defined in the implementation which checks files 
        /// according to the specific process's requirements.</param>
        public bool Check_Files(DelRetArray<bool, string> checkFiles);
        /// <summary>
        /// Promise that an unzipping ETLProcess class will set the temporary location where files are unzipped.
        /// </summary>
        /// <param name="filename"></param>
        void SetTempDir(string filename);

        /// <summary>
        /// Accessor for guid.
        /// </summary>
        public Guid guid { get; }
    }
}
