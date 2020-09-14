using System;
using System.IO;

namespace ETLProcess.General.IO
{
    internal class IOFiles {
        public static readonly string AssemblyDirectory = Directory.GetCurrentDirectory();
        public static Guid PrepGuid { get; } = Guid.NewGuid();
        public static string TempLocation = string.Format($"{AssemblyDirectory}\\{PrepGuid}");
        /// <summary>
        /// Set the temporary location where files are unzipped.
        /// </summary>
        public static void SetTempDir() {
            int i = 0;
            while (Directory.Exists(TempLocation))
            {
                TempLocation += @"_" + i;
                i++;
            }
            Directory.CreateDirectory(TempLocation);
        }
    }
}