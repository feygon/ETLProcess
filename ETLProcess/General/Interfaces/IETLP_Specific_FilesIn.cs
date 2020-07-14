﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ETLProcess.General.Containers.AbstractClasses;

namespace ETLProcess.General.Interfaces
{
    /// <summary>
    /// Interface to promise the structure of an ETLProcess which specifically receives files from a ZipFile.
    /// </summary>

    public interface IETLP_Specific_FilesIn<T_IETLP> : IETLP_Specific<T_IETLP> 
        where T_IETLP: IOProfile<T_IETLP>, new()
    {
        /// <summary>
        /// Promise of an accessor for a lambda to verify the format of each files, checking
        ///  proper naming and integrity requirements.
        /// </summary>
        public DelRetArray<bool, string> CheckFiles_Delegate { get; }

        /// <summary>
        /// Promise that ETLProcesses specific to receiving files as input will provide a way to identify records from those files.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public RecordType IdentifyRecordFile(string filename);

        /// <summary>
        /// Promise that specific ETLProcesses will provide an order for their files to be parsed in.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public Queue<string> OrderFileList(string[] files);

    }
}
