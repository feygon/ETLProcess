using System;
using System.CodeDom;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ETLProcessFactory.Containers.AbstractClasses;
using ETLProcessFactory.Profiles;
using ETLProcessFactory.IO;
using ETLProcessFactory.Containers;

namespace ETLProcessFactory.Interfaces.Profile_Interfaces
{
    /// <summary>
    /// Promise that a class will implement methods specific to outputting data to a database in T-SQL
    /// </summary>
    public interface IExportable_SQL_TSQL<Singleton, T_DataTableClass> : IIExportable_ToSQL
        where Singleton : SingletonProfile<Out_SQLBulkProfile<T_DataTableClass>> 
        where T_DataTableClass : DataTable
    {
        /// <summary>
        /// Promise of a method to initialize IO_SQLOut, instantiate its internal SQLBulkCopy, and Export it.
        /// <para>Note: It is recommended to simply return <see cref="Out_SQLBulkProfile.sqlBulkCopy"/> for this.</para>
        /// </summary>
        /// <param name="dataSet"></param>
        public void ExportSQLOutput(T_DataTableClass dataTable);
    }
}
