using Azure.Data.Tables;
using KickerEloBackend.Models.DatabaseModels;
using System.Linq;

namespace KickerEloBackend.Models.Helpers
{
    internal static class ClientHelper
    {
        public static Client GetClient(string token, TableServiceClient tableService)
        {
            var client = tableService.GetTableClient(DatabaseTables.ClientsTable).Query<Client>(x => x.ClientToken == token).First();

            return client;
        }
    }
}
