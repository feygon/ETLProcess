using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using BasicPreprocess.General.IO;
using BasicPreprocess.General.Interfaces;

namespace BasicPreprocess.General
{

    /// <summary>
    /// A program class for preprocesses that input zipped files, and output xml to Uluro.
    /// </summary>
    public class XMLPreProcess_Takes_Files : IPreP
    {
        /// <summary>
        /// The filenames extracted from the zipfiles.
        /// </summary>
        private string[] Files;

        /// <summary>
        /// Debug Log accessor/mutator, required by interface.
        /// </summary>
        public Log Debug
        {
            get { return Program.Debug; }
            set { Program.Debug = value; }
        }

        /// <summary>
        /// Constructor, takes command line arguments where 0 is the zipfile.
        /// </summary>
        /// <param name="arg">Command line arguments, where index 0 is the zipfile.</param>
        public XMLPreProcess_Takes_Files(string arg)
        {
            SetTempDir(arg);
            SetDebug(arg);
            files = ZipFiles.GetFiles(arg);
        }

        /// <summary>
        /// Filenames from zipped files.
        /// <br>required by interface.</br>
        /// </summary>
        public string[] files
        {
            get { return Files; }
            set { Files = value; }
        }

        /// <summary>
        /// Set the location of the Debug Log
        /// </summary>
        /// <param name="filename">Filename of the zipFile of atrio documents to be processed.</param>
        public void SetDebug(string filename)
        {
            Debug = new Log($@"{Program.AssemblyDirectory}\{DateTime.UtcNow.Ticks}{
                    Path.GetFileNameWithoutExtension(filename)
                }_Debug.log");
        }

        /// <summary>
        /// Run the implementation's checkFiles function.
        /// </summary>
        /// <param name="checkFiles"></param>
        public bool Check_Files (DelRet<bool, string> checkFiles)
        {
            return checkFiles(files);
        }

        /// <summary>
        /// Set the temporary location where files are unzipped.
        /// </summary>
        /// <param name="filename"></param>
        public void SetTempDir(string filename)
        {
            ZipFiles._TempLocation = $@"{Path.GetTempPath()}MetPrep_{DateTime.UtcNow.Ticks}{Path.GetFileNameWithoutExtension(filename)}";
            while (Directory.Exists(ZipFiles._TempLocation))
            {
                ZipFiles._TempLocation += @"_2";
            }
        }
    }
}
