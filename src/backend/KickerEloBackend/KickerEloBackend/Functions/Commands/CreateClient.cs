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

            using var conn = SqlHelper.GetSqlConnection();

            var newClientGuid = Guid.NewGuid().ToString();

            var newClient = await ClientHelper.InsertNewClient(client.ClientName, newClientGuid, conn);

            return new OkObjectResult(newClient);
        }
    }
}
