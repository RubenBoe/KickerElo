

using Microsoft.Data.SqlClient;
using System;

namespace KickerEloBackend.Models.Helpers
{
    internal class SqlHelper
    {
        internal static SqlConnection GetSqlConnection()
        {
            var conn = new SqlConnection(Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING"));
            conn.Open();

            return conn;
        }
    }
}
