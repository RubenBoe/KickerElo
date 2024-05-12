using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using KickerEloBackend.Models.Helpers;
using KickerEloBackend.Models.DatabaseModels;
using KickerEloBackend.Models;
using System.Linq;
using KickerEloBackend.Models.Results;
using System.Web.Http;

namespace KickerEloBackend.Functions.Queries
{
    public static class GetPlayers
    {
        [FunctionName("GetPlayers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Players/{ClientToken}")] HttpRequest req, string ClientToken,
            ILogger log)
        {
            try
            {
                var tableService = TablesHelper.GetTableServiceClient();
                var client = ClientHelper.GetClient(ClientToken, tableService);
                var seasons = tableService.GetTableClient(DatabaseTables.SeasonsTable).Query<Season>(x => x.ClientID == client.Id);
                var currentSeason = seasons.First(s => s.EndDate == null);

                log.LogInformation(currentSeason.SeasonID);

                var players = tableService.GetTableClient(DatabaseTables.PlayersTable).Query<Player>(x => x.ClientID == client.Id).ToList();
                var playerElos = tableService.GetTableClient(DatabaseTables.PlayerEloTable).Query<PlayerElo>(x => x.SeasonID == currentSeason.SeasonID).ToList();

                log.LogInformation("Found players", players, playerElos);

                var result = players
                    .Select(p =>
                    {
                        var correspondingPlayerElo = playerElos.First(x => x.PlayerID == p.PlayerID);
                        return new PlayerResult()
                        {
                            Nickname = p.Nickname,
                            PlayerID = p.PlayerID,
                            EloNumber = correspondingPlayerElo.EloNumber,
                            LastUpdated = correspondingPlayerElo.LastUpdated,
                        };
                    })
                    .OrderByDescending(x => x.EloNumber);

                log.LogInformation("created result", result);

                return new OkObjectResult(result);
            } catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                return new InternalServerErrorResult();
            }
        }
    }
}
