using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using ETLProcess.General.IO;
using ETLProcess.General.Interfaces;

namespace ETLProcess.General
{

    /// <summary>
    /// A program class for ETLProcesses that input zipped files, and output xml to Uluro.
    /// </summary>
    public class FilesIn_XMLOut : IETLP
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
        /// Each type of ETLProcess needs one of these.
        /// </summary>
        private static Guid prepGuid;

        /// <summary>
        /// Initialize this ETLProcess's unique identifier.
        /// </summary>
        public static Guid PrepGuid { get
            {
                if (prepGuid == null)
                {
                    prepGuid = Guid.NewGuid();
                }
                return prepGuid;
            } 
        }

        /// <summary>
        /// Unique id for this ETLProcess.
        /// </summary>
        public Guid guid { get { return prepGuid; } }

        /// <summary>
        /// Constructor, takes command line arguments where 0 is the zipfile.
        /// </summary>
        /// <param name="arg">Command line arguments, where index 0 is the zipfile.</param>
        public FilesIn_XMLOut(string arg)
        {
            SetTempDir(arg);
            InitLog(arg);
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
        /// Set the location of the Debug Log, for use in the constructor.
        /// </summary>
        /// <param name="filename">Filename of the zipFile of Client documents to be processed.</param>
        public void InitLog(string filename)
        {
            Debug = new Log($@"{Program.AssemblyDirectory}\{prepGuid}{
                    Path.GetFileNameWithoutExtension(filename)
                }_Debug.log");
        }

        /// <summary>
        /// Run the implementation's checkFiles function.
        /// </summary>
        /// <param name="checkFiles"></param>
        public bool Check_Files (DelRetArray<bool, string> checkFiles)
        {
            return checkFiles(files);
        }

        /// <summary>
        /// Set the temporary location where files are unzipped.
        /// </summary>
        /// <param name="filename"></param>
        public void SetTempDir(string filename)
        {
            ZipFiles._TempLocation = $@"{Path.GetTempPath()}MetPrep_{PrepGuid}{Path.GetFileNameWithoutExtension(filename)}";
            int i = 0;
            while (Directory.Exists(ZipFiles._TempLocation))
            {
                ZipFiles._TempLocation += @"_" + i;
                i++;
            }
        }
    }
}
