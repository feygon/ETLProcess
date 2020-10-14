using System;
using System.IO;
using System.Diagnostics;
using ETLProcess.General.IO;
using System.ComponentModel;

namespace ETLProcess.General.IO
{
    /// <summary>
    /// A class for logging and outputting debug messages.
    /// </summary>
    internal sealed class Log
    {
        public static Log Instance { get; } = new Log(); // Singleton Constructor.
        public static string logFileName = "";
        /// <summary>
        /// Constructor for singleton output log
        /// </summary>
		private Log() { }

        private static FileInfo logFileInfo;
        /// <summary>
        /// Access this to write a line with the writeline command.
        /// </summary>
		private static StreamWriter Writer;
        /// <summary>
        /// The writer to write a line with the writeline command.
        /// </summary>
        public static StreamWriter writer
        {
            get
            {
                var threadlock = new object();
                lock (threadlock)
                {
                    if (Writer == null) { throw new NullReferenceException(); }
                }
                return Writer;
            }
            private set {
                var threadlock = new object();
                lock (threadlock)
                {
                    Writer = value;
                }
            }
        }

        /// <summary>
        /// Set the location of the Debug Log, for use in the constructor.
        /// </summary>
        /// <param name="filename">Filename of the zipFile of Client documents to be processed.</param>
        public static void InitLog(string filename = null)
        {
            IOFiles.SetTempDir();
            logFileName = string.Format($"{IOFiles.TempLocation}\\{Path.GetFileNameWithoutExtension(filename)}.log");
            writer = File.CreateText(logFileName);
            logFileInfo = new FileInfo(logFileName);

            //return new Log($@"{IOFiles.TempLocation}\{filename ??= "Process"}_Debug.log");
        }

        /*****Operation Methods*****/

        /// <summary>
        /// Write to the output log
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
		{
            Debug.Write(message);
            writer.WriteLine($"- - - - - - - - - - {DateTime.Now.ToLongTimeString()}\r\n{message}\r\n\r\n");
			writer.Flush();
		}

        public static void WriteWarningException(string message, WarningException err = null) {
            Write(string.Format($"Warning: {message}"));
            if (err != null)
            {
                throw new WarningException(message, err);
            } else {
                throw new WarningException(message);
            }
        }

        public static void WriteException(string message, Exception err = null)
        {
            Write(message);
            if (err != null) {
                throw new Exception(message, err);
            } else {
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Dispose of a log.
        /// </summary>
        public static void Remove()
        {
            writer.Dispose();
            logFileInfo.Delete();
        }
	}
}
