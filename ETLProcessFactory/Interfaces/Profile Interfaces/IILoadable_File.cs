using ETLProcess;
using ETLProcessFactory.GP;
using ETLProcessFactory.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETLProcessFactory.Interfaces
{
    /// <summary>
    /// Interface to promise the structure of an ETLProcess which specifically receives files from a ZipFile.
    /// </summary>

    public interface IILoadable_File : IIILoadable
    {
        /// <summary>
        /// Promise of an accessor for a lambda to verify the format of each files, checking
        ///  proper naming and integrity requirements.
        /// </summary>
        public DelRet<bool, string[]> CheckFiles_Delegate { get; }

        /// <summary>
        /// Promise that ETLProcesses specific to receiving files as input will provide a way to identify records from those files (Implementation-specific enum recommended).
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public int IdentifyRecordFile(string filename);

        /// <summary>
        /// Promise that specific ETLProcesses will provide an order for their files to be parsed in.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public Queue<string> OrderFileList(string[] files);
    }
}