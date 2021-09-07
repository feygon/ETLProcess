using System;
using System.IO;

namespace BasicPreprocess.General
{
    /// <summary>
    /// A class for logging and outputting debug messages.
    /// </summary>
    public class Log : ILog
    {
        private readonly FileInfo logFileInfo;
        private readonly StreamWriter writer;

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
        public void Write(string message)
        {
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
