using System.Linq;
using System.Data;
using System.Collections.Generic;

using ETLProcess.General.IO;
using System.Reflection;
using ETLProcess.General.Interfaces;
using ETLProcess.General.Containers.AbstractClasses;
using System;
using System.ComponentModel;

namespace ETLProcess.General.Profiles
{

    /// <summary>
    /// A program class for ETLProcesses that input zipped files, and output xml.
    /// </summary>
    public class IO_FilesIn : SingletonProfile<IO_FilesIn>, IDisposable {
        /// <summary>
        /// Filenames from zipped files.
        /// <br>required by interface.</br>
        /// </summary>
        public string[] Files {
            get {
                if (!firstRun) { 
                    return files; 
                } else {
                    throw new System.Exception("First run instance should not be accessed. " +
                        "Try calling static instance SingletonProfile<IO_FilesIn>.Instance.< member name > instead.");
                }
            }
            set { files = value; }
        }
        private string[] files;
        private static bool filesGood = false;
        private static bool filesChecked = false;
        /*****Constructor*****/

        /// <summary>
        /// Constructor, takes command line arguments where 0 is the zipfile.
        /// </summary>
        /// <param name="arg">Command line arguments, where index 0 is the zipfile.</param>
        public IO_FilesIn(string arg) 
            : base(typeof(IO_FilesIn)
                  , new object[]{ arg }) 
        {
            if (!firstRun) {
                Files = ZipFiles.GetFiles(arg).Where((x) => x != Log.logFileName).ToArray();
            }
        }
        /// <summary>
        /// Public parameterless constructor. For inheritance use only.
        /// </summary>
        public IO_FilesIn() : base(typeof(IO_FilesIn), null) { }

        /// <summary>
        /// Run the implementation's checkFiles function.
        /// </summary>
        /// <param name="checkFiles">Delegate to check integrity of files.</param>
        public bool Check_Input(DelRet<bool, string[]> checkFiles) {
            if (!filesChecked) {
                filesGood = checkFiles(Files);
                if (filesGood) { Log.Write("Files approved, by checkFiles Delegate."); }
            }
            return filesGood;
        }

        /// <summary>
        /// A method to dispose of the initial instance.
        /// </summary>
        public void Dispose() {
            if (firstRun) {
                disposable.Dispose();
                GC.SuppressFinalize(this);
            } else { 
                Log.WriteException("Dispose called on singleton or non-initial instance."); 
            }
        }
    }
}
