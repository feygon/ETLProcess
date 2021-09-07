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
    public interface iPreProcess_From_Zip
    {
        /// <summary>
        /// Must implement debug log somewhere.
        /// </summary>
        Log Debug { set; get; }

        /// <summary>
        /// Must implement files somewhere.
        /// </summary>
        string[] Files { set; get; }

        /// <summary>
        /// Implements Debugging log with path for logging to go.
        /// </summary>
        /// <param name="filename">Filename of the zipfile in question.</param>
        void SetDebug(string filename);
    }
}
