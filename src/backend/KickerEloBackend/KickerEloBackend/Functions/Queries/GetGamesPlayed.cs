using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KickerEloBackend.Models.Helpers;
using KickerEloBackend.Models;
using KickerEloBackend.Models.DatabaseModels;
using KickerEloBackend.Models.Results;
using System.Linq;

namespace KickerEloBackend.Functions.Queries
{
    public static class GetGamesPlayed
    {
        [FunctionName("GetGamesPlayed")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Games/{SeasonID}")] HttpRequest req, string SeasonID,
            ILogger log)
        {
            using var conn = SqlHelper.GetSqlConnection();

            var games = await GameHelper.GetSeasonGames(SeasonID, conn);
            
            var gamesPlayed = await GameHelper.GetGameResults(games, conn);

            return new OkObjectResult(gamesPlayed);
        }
    }
}
