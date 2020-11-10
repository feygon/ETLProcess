namespace ETLProcess.General.Interfaces.Profile_Interfaces
{
    /// <summary>
    /// Interface to promise the structure of an ETLProcess which specifically produces output SQL queries.
    /// </summary>
    public interface _IOutputMode_SQL : _IOutput_Profile
    {
        /// <summary>
        /// Promise that a class implementing SQL output with this interface will implement a method
        ///     to return a lambda to check the SQL output for validity.
        /// </summary>
        /// <param name="options">Optional parameters for creating the lambda.</param>
        /// <returns></returns>
        public DelRet<bool> GetCheck_SQL_Output(object[] options);
    }
}
