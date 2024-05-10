using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Azure.Data.Tables;
using System;
using System.Web.Http;
using KickerEloBackend.Models;
using KickerEloBackend.Models.Helpers;

namespace KickerEloBackend.Functions
{
    public static class Status
    {
        [FunctionName("Status")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            TableServiceClient client = TablesHelper.GetTableServiceClient();

            TableClient clientsTable = client.GetTableClient(
                tableName: DatabaseTables.ClientsTable
            );
            await clientsTable.CreateIfNotExistsAsync();

            TableClient seasonsTable = client.GetTableClient(
                tableName: DatabaseTables.SeasonsTable
            );
            await seasonsTable.CreateIfNotExistsAsync();

            var playersTable = client.GetTableClient(
                tableName: DatabaseTables.PlayersTable
                );
            await playersTable.CreateIfNotExistsAsync();

            var gamesTable = client.GetTableClient(
                tableName: DatabaseTables.GamesTable);
            await gamesTable.CreateIfNotExistsAsync();

            var playerGameTable = client.GetTableClient(
                tableName: DatabaseTables.PlayerGameTable);
            await playerGameTable.CreateIfNotExistsAsync();

            var playerEloTable = client.GetTableClient(
                tableName: DatabaseTables.PlayerEloTable);
            await playerEloTable.CreateIfNotExistsAsync();




            return new OkObjectResult("Service running");
        }
    }
}
