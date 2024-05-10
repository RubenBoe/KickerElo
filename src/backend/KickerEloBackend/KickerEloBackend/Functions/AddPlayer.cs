using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using KickerEloBackend.Models.CommandModels;
using KickerEloBackend.Models.Helpers;
using KickerEloBackend.Models.DatabaseModels;
using KickerEloBackend.Models;
using System.Linq;

namespace KickerEloBackend.Functions
{
    public static class AddPlayer
    {
        [FunctionName("AddPlayer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<AddPlayerCommand>(requestBody);

            var tableService = TablesHelper.GetTableServiceClient();
            var client = ClientHelper.GetClient(data.ClientToken, tableService);

            var newPlayerId = Guid.NewGuid().ToString();
            var newPlayer = new Player()
            {
                ClientID = client.Id,
                Nickname = data.Nickname,
                FullName = data.FullName,
                PlayerID = newPlayerId,
                RowKey = newPlayerId
            };

            await tableService.GetTableClient(DatabaseTables.PlayersTable).AddEntityAsync(newPlayer);

            var currentSeason = tableService.GetTableClient(DatabaseTables.SeasonsTable).Query<Season>(x => x.EndDate == null && x.ClientID == client.Id).First();

            var newPlayerElo = new PlayerElo(newPlayerId, currentSeason.SeasonId, EloHelper.InitialEloNumber);

            await tableService.GetTableClient(DatabaseTables.PlayerEloTable).AddEntityAsync(newPlayerElo);

            return new OkObjectResult(newPlayer);
        }
    }
}
