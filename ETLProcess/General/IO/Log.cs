using System;
using System.IO;
using System.Diagnostics;

namespace ETLProcess.General
{
    /// <summary>
    /// A class for logging and outputting debug messages.
    /// </summary>
    public class Log
    {
        private readonly FileInfo logFileInfo;
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
        /// Constructor for output log
        /// </summary>
        /// <param name="directory"></param>
		public Log(string directory)
		{
			writer = File.CreateText(directory);
            logFileInfo = new FileInfo(directory);
        }

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

        /// <summary>
        /// Dispose of a log.
        /// </summary>
        public void Remove()
        {
            writer.Dispose();
            logFileInfo.Delete();
        }
	}
}
