using Azure.Data.Tables;
using Dapper;
using KickerEloBackend.Models.DatabaseModels;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KickerEloBackend.Models.Helpers
{
    internal static class ClientHelper
    {
        public async static Task<Client> GetClient(string token, SqlConnection connection)
        {
            var client = await connection.QuerySingleAsync<Client>($@"
            SELECT ClientName, CreationDate, Id, ClientToken
            FROM clients
            WHERE ClientToken=@ClientToken", new { ClientToken = token });

            return client;
        }

        public async static Task<Client> InsertNewClient (string clientName, string clientToken, SqlConnection connection)
        {
            var client = await connection.QuerySingleAsync<Client>($@"
            INSERT INTO clients (ClientName, ClientToken, CreationDate)
                OUTPUT Inserted.Id, inserted.ClientName, inserted.ClientToken, inserted.CreationDate
                VALUES (@clientName, @clientToken, GETUTCDATE())", new { clientName, clientToken });

            return client;
        }
    }
}
