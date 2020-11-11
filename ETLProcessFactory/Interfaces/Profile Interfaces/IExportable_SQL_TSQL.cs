using System;
using System.CodeDom;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ETLProcessFactory.Containers.AbstractClasses;
using ETLProcessFactory.Profiles;
using ETLProcessFactory.IO;

namespace ETLProcessFactory.Interfaces.Profile_Interfaces
{
    /// <summary>
    /// Promise that a class will implement methods specific to outputting data to a database in T-SQL
    /// </summary>
    public interface IExportable_SQL_TSQL<Singleton, TOutput> : IIExportable_ToSQL
        where Singleton : SingletonProfile<IO_SQLOut>
        where TOutput : ISerializable
    {
        /// <summary>
        /// Promise of a method which formats a TSQL Query.
        /// </summary>
        /// <param name="outputDocs">A list of serializable output documents, to bulk copy?</param>
        public void TSQLFormat(List<TOutput> outputDocs);

        /// <summary>
        /// Promise of a method that returns a SQLBulkCopy object, with its dataset.
        /// </summary>
        /// <param name="dataSet"></param>
        public SqlBulkCopy Get_BulkSQLCopy(DataSet dataSet);
    }
}
