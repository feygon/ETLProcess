using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using ETLProcess.General.IO;
using ETLProcess.General.Interfaces;
using System.Data;

using ETLProcess.General.Containers.AbstractClasses;
using System.Runtime.Serialization;

namespace ETLProcess.General.Profiles
{

    /// <summary>
    /// A program class for ETLProcesses that input zipped files, and output xml.
    /// </summary>
    public class FilesIn_XMLOut : IOProfile<FilesIn_XMLOut>
    {
        /// <summary>
        /// Default constructor, for conversion only.
        /// </summary>
        public FilesIn_XMLOut() : base() { }

        /*****Constructor*****/

        /// <summary>
        /// Constructor, takes command line arguments where 0 is the zipfile.
        /// </summary>
        /// <param name="arg">Command line arguments, where index 0 is the zipfile.</param>
        public FilesIn_XMLOut(string arg) : base(ZipFiles.GetFiles(arg))
        {
            SetTempDir(arg);
            InitLog(arg);
        }

        /*****Override Methods*****/

        /// <summary>
        /// Run the implementation's checkFiles function.
        /// </summary>
        /// <param name="checkFiles"></param>
        public override bool Check_Files(DelRetArray<bool, string> checkFiles)
        {
            return checkFiles(files);
        }

        /// <summary>
        /// Set the temporary location where files are unzipped.
        /// </summary>
        /// <param name="filename"></param>
        public override void SetTempDir(string filename)
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
