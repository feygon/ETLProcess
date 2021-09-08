namespace BasicPreprocess.General
{
    public interface ILog
    {
        /// <summary>
        /// Remove a message from log.
        /// </summary>
        void Remove();
        
        /// <summary>
        /// Write a message to log.
        /// </summary>
        /// <param name="message"></param>
        void Write(string message);
    }
}