using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Data;
using Microsoft.VisualBasic.FileIO;

namespace ETLProcess.General.IO
{
    internal sealed class ZipFiles : IOFiles
    {
        private const string _7ZipExecutable = @"C:\Program Files\7-Zip\7z.exe";

        /// <summary>
        /// Uses 7Zip to extract the filePath to the _TempLocation. If the extract fails then it is assumed the file is not an archive
        /// and will just be copied to _TempLocation for processing
        /// </summary>
        /// <param name="filePath">The full path of the file to extract</param>
        public static void Extract(string filePath)
        {

            // Confirm that arg 0 is a qualified file path
            var inputFile = new FileInfo(filePath);
            if (!inputFile.Exists)
            {
                throw new Exception("Input file does not exist");
            }
            // Open a new process to attempt a 7zip archive extract to a temp folder
            // The process must execute and wait for completion
            using var process = new Process();
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = $"{@"/C"} \"\"{_7ZipExecutable}\" {@"e -bd -y -o"}\"{TempLocation}\" \"{filePath}\"\""
            };
            Log.Write($"Executing: {startInfo.FileName} {startInfo.Arguments}");
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            // Log the 7zip results
            Log.Write("7zip result code: " + process.ExitCode);
            // Assume that 7zip errors mean that the file is not a zip archive and process as such
            if (process.ExitCode != 0)
            {
                Log.Write("Not an archive? Processing as single file.");
                Directory.CreateDirectory(TempLocation);
                File.Copy(filePath, Path.Combine(TempLocation, inputFile.Name));
            }
        }

        /// <summary>
        /// Get files from zipped archive into temporary directory, using command line arguments.
        /// </summary>
        /// <param name="filepath">The path of a zipfile where the files are temporarily stored.</param>
        /// <returns></returns>
        public static string[] GetFiles(string filepath)
        {
            // Begin the process
            Log.Write("ETLProcess Begun");
            // Handle 7zip extraction
            ZipFiles.Extract(filepath);

            // Begin file processing on files in the temp location
            return Directory.GetFiles(TempLocation);
        }
    }
}
