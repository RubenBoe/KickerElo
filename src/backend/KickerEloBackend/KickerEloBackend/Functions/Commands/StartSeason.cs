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
using KickerEloBackend.Models;
using KickerEloBackend.Models.DatabaseModels;
using System.Linq;
using Azure;

namespace KickerEloBackend.Functions.Commands
{
    public static class StartSeason
    {
        [FunctionName("StartSeason")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<StartSeasonCommand>(requestBody);

            var tableService = TablesHelper.GetTableServiceClient();

            var client = ClientHelper.GetClient(data.ClientToken, tableService);

            var seasonsTable = tableService.GetTableClient(DatabaseTables.SeasonsTable);
            var seasons = seasonsTable.Query<Season>(x => x.ClientID == client.Id);

            var newSeasonNumber = 1;
            if (seasons.Any())
            {
                var currentSeason = seasons.First(s => s.EndDate == null);

                // end current season
                currentSeason.EndDate = DateTime.UtcNow;
                await seasonsTable.UpdateEntityAsync(currentSeason, ETag.All);

                newSeasonNumber = currentSeason.SeasonNumber + 1;
            }

            var newSeasonId = $"{client.Id}_{newSeasonNumber}";
            var newSeason = new Season()
            {
                SeasonNumber = newSeasonNumber,
                SeasonId = newSeasonId,
                StartDate = DateTime.UtcNow,
                ClientID = client.Id,
                RowKey = newSeasonId
            };
            await seasonsTable.AddEntityAsync(newSeason);

            // Reset Elo numbers of all players for new season
            var players = tableService.GetTableClient(DatabaseTables.PlayersTable).Query<Player>(p => p.ClientID == client.Id);
            foreach (var player in players)
            {
                var newPlayerElo = new PlayerElo(player.PlayerID, newSeasonId, EloHelper.InitialEloNumber);
                await tableService.GetTableClient(DatabaseTables.PlayerEloTable).AddEntityAsync(newPlayerElo);
            }

            return new OkObjectResult(newSeason);
        }
    }
}
