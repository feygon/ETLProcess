using ETLProcess;
using ETLProcessFactory.Containers.Dictionaries;
using ETLProcessFactory.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalCoreLib;

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
        /// Promise of a place to put the order of the file list, promised as a return for <see cref="OrderFileList(string[], out Queue{string})"/>
        /// </summary>
        public Queue<string> FileListOrder { get; }

        /// <summary>
        /// Promise that specific ETLProcesses will provide an order for their files to be parsed in.
        /// </summary>
        /// <param name="files">Files to order by filename</param>
        /// <param name="FileListOrder">Files in order, in a queue of filenames, to put into <see cref="FileListOrder"/></param>
        /// <returns></returns>
        public IClient OrderFileList(string[] files, out Queue<string> FileListOrder);
    }
}