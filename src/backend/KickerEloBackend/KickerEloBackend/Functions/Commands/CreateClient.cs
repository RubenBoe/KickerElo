using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using KickerEloBackend.Models.DatabaseModels;
using Azure.Data.Tables;
using System;
using KickerEloBackend.Models;
using System.Linq;
using KickerEloBackend.Models.Helpers;

namespace KickerEloBackend.Functions.Commands
{
    public static class CreateClient
    {
        [FunctionName("CreateClient")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var client = JsonSerializer.Deserialize<Client>(requestBody);

            TableServiceClient tableServiceClient = TablesHelper.GetTableServiceClient();

            var clientsTable = tableServiceClient.GetTableClient(DatabaseTables.ClientsTable);
            var currentClients = clientsTable.Query<Client>(x => true);
            var currentHighestID = currentClients.Count() > 0 ? currentClients.Select(c => c.Id).Max() : 0;

            // Set new client's ID to the highest one available
            client.Id = currentHighestID + 1;
            client.RowKey = client.Id.ToString();
            var newClientGuid = Guid.NewGuid().ToString();
            client.ClientToken = newClientGuid;

            await clientsTable.AddEntityAsync(client);

            return new OkObjectResult(client);
        }
    }
}
