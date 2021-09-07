﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace BasicPreprocess
{
    internal static class SQL
    {
        internal static SqlConnection UluroConnection { get; } =
            new SqlConnection(@"Data Source=ULURO-SQL\ULURO;Initial Catalog=uluroweb30;User id=trans;Password=trans;");

        /// <summary>
        /// Queries a SQL server and executes a command, returning a DataTable of the results or null in case of an error. 
        /// Also handles the error processing in case of errors.
        /// </summary>
        /// <param name="connectionString">A string of connection information for the SQL server you would like to query</param>
        /// <param name="commandString">A string of the SQL query which you would like to execute against the SQL server</param>
        /// <returns>A new DataTable object populated with the results of the query commandString. Returns null in case of an error.</returns>
        internal static DataTable ExecuteSQLCommand(string connectionString, string commandString)
        {
            using var returnable = new DataTable();
            using var SQLConnect = new SqlConnection(connectionString);
            using var SQLCommand = new SqlCommand(commandString, SQLConnect);
            using var SQLReader = new SqlDataAdapter(SQLCommand);
            returnable.Locale = CultureInfo.InvariantCulture;
            // Open the connection and read in data
            SQLConnect.Open();
            SQLReader.Fill(returnable);
            foreach (DataRow row in returnable.Rows)
            {
                foreach (DataColumn column in returnable.Columns)
                {
                    if (row[column.ColumnName] == DBNull.Value)
                    {
                        row[column.ColumnName] = string.Empty;
                    }
                    else if (column.DataType == Type.GetType("System.String"))
                    {
                        row[column.ColumnName] = ((string)row[column.ColumnName]).Trim();
                    }
                }
            }
            return returnable;
        }

        /// <summary>
        /// Queries a SQL server and executes a command, returning a DataTable of the results or null in case of an error. 
        /// Also handles the error processing in case of errors.
        /// </summary>
        /// <param name="command">The compiled command to execute</param>
        /// <returns>A new DataTable object populated with the results of the query commandString. Returns null in case of an error.</returns>
        internal static DataTable ExecuteBuiltCommand(SqlCommand command)
        {
            using var returnable = new DataTable();
            using var reader = new SqlDataAdapter(command);
            // Open the connection and read in data
            command.Connection.Open();
            command.Prepare();
            returnable.Locale = CultureInfo.InvariantCulture;
            reader.Fill(returnable);
            foreach (DataRow row in returnable.Rows)
            {
                foreach (DataColumn column in returnable.Columns)
                {
                    if (column.DataType != Type.GetType("System.String"))
                        continue;
                    if (row[column.ColumnName] == DBNull.Value)
                    {
                        row[column.ColumnName] = string.Empty;
                    }
                    else
                    {
                        row[column.ColumnName] = ((string)row[column.ColumnName]).Trim();
                    }
                }
            }
            command.Connection.Close();
            return returnable;
        }

        /// <summary>
        /// Queries a SQL server and executes a command which will not return a table of results.
        /// </summary>
        /// <param name="command">The compiled command to execute</param>
        /// <returns>An integer representing the SQL error response code</returns>
        internal static int ExecuteBuiltCommandNonQuery(SqlCommand command)
        {
            // Open the connection and read in data
            command.Connection.Open();
            int res = command.ExecuteNonQuery();
            command.Connection.Close();
            return res;
        }
    }
}
