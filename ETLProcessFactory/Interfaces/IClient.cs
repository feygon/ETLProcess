using System.Collections.Generic;

namespace ETLProcessFactory.Interfaces
{
    /// <summary>
    /// Interface to provide chained methods in the client public class.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Step 1 -- Load records from a source, I.E. an XMLTree, a delimited file, or a SQL Query <br/>
        /// Redundant with <see cref="IIILoadable"/>
        /// </summary>
        /// <returns></returns>
        public IClient PopulateRecords();
        /// <summary>
        /// Step 2 -- Process Records<br/>
        /// See Step 1 in <see cref="IIILoadable"/>
        /// </summary>
        /// <returns></returns>
        public IClient ProcessRecords();
        /// <summary>
        /// Step 3 -- Export Reports via the client's reporting method, if any.
        /// </summary>
        /// <returns></returns>
        public IClient ExportReports();

        /// <summary>
        /// Step 0? -- Order file list.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="files"></param>
        /// <param name="fileListOrder"></param>
        /// <returns></returns>
        public IClient OrderFileList(string[] files, out Queue<string> fileListOrder);
    }
}