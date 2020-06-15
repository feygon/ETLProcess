using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProcess.General.Interfaces
{


    /// <summary>
    /// Interface to promise the structure of an ETLProcess which receives files from a ZipFile.
    /// </summary>
    public interface IETLP_Specific<T_IETLP> where T_IETLP : IETLP
    {
        /// <summary>
        /// Promise of an accessor for Key Columns of each class (not including indexers).
        /// </summary>
        public Dictionary<Type, List<string>> keyColumns
        {
            get;
        }

        /// <summary>
        /// Promise of an accessor for a lambda to verify the format of each files, checking
        ///  proper naming and integrity requirements.
        /// </summary>
        public DelRetArray<bool, string> CheckFiles_Delegate { get; }

        /// <summary>
        /// Promise that a specific implementation will have a way to populate docs using members in the client object.
        /// </summary>
        public void PopulateRecords();

        /// <summary>
        /// Promise that specific ETLProcesses will provide a way to identify document types.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public DocType IdentifyRecordFile(string filename);

        /// <summary>
        /// Promise that specific ETLProcesses will provide an order for their files to be parsed in.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public Queue<string> OrderFileList(string[] files);
    }
}
