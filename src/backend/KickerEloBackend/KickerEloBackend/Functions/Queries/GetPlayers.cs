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

namespace KickerEloBackend.Functions.Queries
{
    public static class GetPlayers
    {
        [FunctionName("GetPlayers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Players/{ClientToken}")] HttpRequest req, string ClientToken,
            ILogger log)
        {
            var tableService = TablesHelper.GetTableServiceClient();
            var client = ClientHelper.GetClient(ClientToken, tableService);
            var currentSeason = tableService.GetTableClient(DatabaseTables.SeasonsTable).Query<Season>(x => x.ClientID == client.Id && x.EndDate == null).First();

            var players = tableService.GetTableClient(DatabaseTables.PlayersTable).Query<Player>(x => x.ClientID == client.Id);
            var playerElos = tableService.GetTableClient(DatabaseTables.PlayerEloTable).Query<PlayerElo>(x => x.SeasonID == currentSeason.SeasonID);

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


            return new OkObjectResult(result);
        }
    }
}
