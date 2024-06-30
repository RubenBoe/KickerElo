using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KickerEloBackend.Models.Helpers;
using KickerEloBackend.Models;
using KickerEloBackend.Models.DatabaseModels;
using System.Linq;
using KickerEloBackend.Models.Results;

namespace KickerEloBackend.Functions.Queries
{
    public static class GetPlayerDetails
    {
        [FunctionName("GetPlayerDetails")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Player/{PlayerID}")] HttpRequest req, string PlayerID,
            ILogger log)
        {
            using var conn = SqlHelper.GetSqlConnection();
            var player = await PlayerHelper.GetPlayerDetails(PlayerID, conn);

            var games = await PlayerHelper.GetGamesForPlayer(PlayerID, conn);

            log.LogInformation($"Found {games.Count()} games for this player");

            var result = new PlayerDetailsResult( player, await GameHelper.GetGameResults(games, conn));

            log.LogInformation($"Calculated result object");

            return new OkObjectResult(result);
        }
    }
}
