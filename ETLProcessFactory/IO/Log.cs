using System;
using System.IO;
using System.Diagnostics;
using ETLProcessFactory.IO;
using System.ComponentModel;

namespace ETLProcessFactory.IO
{
    /// <summary>
    /// A class for logging and outputting debug messages.
    /// </summary>
    public class Log
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
		private static StreamWriter _writer;
        /// <summary>
        /// The writer to write a line with the writeline command.
        /// </summary>
        public static StreamWriter Writer
        {
            get
            {
                var threadlock = new object();
                lock (threadlock)
                {
                    if (_writer == null) { throw new NullReferenceException(); }
                }
                return _writer;
            }
            private set {
                var threadlock = new object();
                lock (threadlock)
                {
                    _writer = value;
                }
            }
        }

        /// <summary>
        /// Set the location of the Debug Log, for use in the constructor.
        /// </summary>
        /// <param name="filename">Filename of the zipFile of Client documents to be processed.</param>
        public static void InitLog(string filename = null)
        {
            IODirectory.SetTempDir();
            logFileName = string.Format($"{IODirectory.TempLocation}\\{Path.GetFileNameWithoutExtension(filename)}.log");
            Writer = File.CreateText(logFileName);
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
            Writer.WriteLine($"- - - - - - - - - - {DateTime.Now.ToLongTimeString()}\r\n{message}\r\n\r\n");
			Writer.Flush();
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
            Writer.Dispose();
#if !DEBUG
            logFileInfo.Delete();
#endif
        }
	}
}
