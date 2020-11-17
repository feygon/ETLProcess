using System.Data;
using System.Data.SqlClient;

using ETLProcessFactory.IO;

namespace ETLProcess.Specific
{
    internal static class ClientSpecificSQL
    {
        /// <summary>
        /// Updates a Customer in Mail Shop with a new assigned customer code
        /// </summary>
        internal static DataTable Execute(string sqlQueryString)
        {
            using var command = new SqlCommand(sqlQueryString, SQL.Conn);
            return SQL.ExecuteBuiltCommandReturnQuery(command);
        }

        // UPDATE Customers with new code
        internal const string getClientAccounts = @"
			SELECT
	            Documents.Account AS MEMBERID,
	            Documents.Misc1 AS ACCOUNTID
            FROM ExampleDB.dbo.Submissions
            LEFT JOIN ExampleDB.dbo.Documents ON Submissions.Submid = Documents.Submid
            LEFT JOIN ExampleDB.dbo.Subtypes ON Submissions.SubtypeID = Subtypes.SubtypeID
            LEFT JOIN ExampleDB.dbo.Customer ON Subtypes.CustID = Customer.CustID
            WHERE Submissions.subtypeid IN (Alpha, Beta)
                AND Submissions.[Status] = 'C'
            GROUP BY Documents.Account, Documents.Misc1
		";
    }
}
