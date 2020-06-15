using System.Data;
using System.Data.SqlClient;

namespace ETLProcess.Specific
{
    internal static class ClientSpecificSQL
    {
        /// <summary>
        /// Updates a Customer in Mail Shop with a new assigned customer code
        /// </summary>
        internal static DataTable Execute(string sqlQueryString)
        {
            using var command = new SqlCommand(sqlQueryString, SQL.UluroConnection);
            return SQL.ExecuteBuiltCommandReturnQuery(command);
        }

        // UPDATE Customers with new code
        internal const string getClientAccounts = @"
			SELECT
	            Documents.Account AS MEMBERID,
	            Documents.Sys_MiscText1 AS ACCOUNTID
            FROM Uluro.dbo.Submissions
            LEFT JOIN Uluro.dbo.Documents ON Submissions.Submid = Documents.Submid
            WHERE Submissions.subtypeid IN (131,373)
                AND Submissions.[Status] = 'C'
            GROUP BY Documents.Account, Documents.Sys_MiscText1
		";
    }
}
