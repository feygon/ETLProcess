using System.Data;
using System.Data.SqlClient;

namespace BasicPreprocess.Specific
{
    internal static class GetATRIOAccounts
    {
        /// <summary>
        /// Updates a Customer in Mail Shop with a new assigned customer code
        /// </summary>
        internal static DataTable Execute()
        {
            using var command = new SqlCommand(commandText, SQL.UluroConnection);
            return SQL.ExecuteBuiltCommand(command);
        }

        // UPDATE Customers with new code
        private const string commandText = @"
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
