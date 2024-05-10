using Azure.Data.Tables;
using System;

namespace KickerEloBackend.Models.Helpers
{
    internal static class TablesHelper
    {
        public static TableServiceClient GetTableServiceClient ()
        {
            TableServiceClient tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("TABLE_CONNECTION_STRING"));

            return tableServiceClient;
        }
    }
}
