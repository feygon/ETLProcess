﻿namespace ETLProcess.General.Interfaces.Profile_Interfaces
{
    /// <summary>
    /// Interface to promise the structure of an ETLProcess which specifically produces output files.
    /// </summary>
    public interface _IOutputMode_Files : _IOutput_Profile
    {
        /// <summary>
        /// Promise that a class implementing file output with this interface will implement a method to
        ///     return a lambda to check that file output,
        ///     for use with IOut_A_OutputProfile's Check_Output method.
        /// </summary>
        /// <param name="options">Optional parameters for creating the lambda.</param>
        /// <returns></returns>
        public DelRet<bool, string[]> GetCheck_File_Output(object[] options);
    }
}