using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ETLProcess.General.IO;
using ETLProcess.General.Interfaces;
using System.Data;

namespace ETLProcess.General.Containers.AbstractClasses
{
    /// <summary>
    /// An abstract basis for ETLProcess classes which input from a certain type,
    ///     and output to a certain type.
    /// </summary>
    public abstract class IOProfile<T> where T: IOProfile<T>, new()
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public IOProfile() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="files">Filenames of files to extract, if any.</param>
        public IOProfile(string[] files)
        {
            this.files = files;
        }

        /// Filenames from zipped files.
        protected string[] Files;
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
        /// Run the implementation's checkFiles function.
        /// </summary>
        /// <param name="checkFiles"></param>
        public abstract bool Check_Files(DelRetArray<bool, string> checkFiles);

        /// <summary>
        /// Set the temporary location where files are unzipped.
        /// </summary>
        /// <param name="filename"></param>
        public abstract void SetTempDir(string filename);

        /// <summary>
        /// Debug Log accessor/mutator, required by interface.
        /// </summary>
        internal static Log Debug
        {
            get { return Program.Debug; }
            set { Program.Debug = value; }
        }

        /// <summary>
        /// Set the location of the Debug Log, for use in the constructor.
        /// </summary>
        /// <param name="filename">Filename of the zipFile of Client documents to be processed.</param>
        public static void InitLog(string filename)
        {
            if (Debug != null)
            {
                Debug = new Log($@"{Program.AssemblyDirectory}\{prepGuid}{
                        Path.GetFileNameWithoutExtension(filename)
                    }_Debug.log");
            }
        }

        /// <summary>
        /// Each type of ETLProcess needs one of these.
        /// </summary>
        protected static Guid prepGuid;

        /// <summary>
        /// Initialize this ETLProcess's unique identifier.
        /// </summary>
        public static Guid PrepGuid
        {
            get
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
    }
}
