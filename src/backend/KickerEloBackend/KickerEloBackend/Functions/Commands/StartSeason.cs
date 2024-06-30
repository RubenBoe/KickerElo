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

            using var conn = SqlHelper.GetSqlConnection();

            var newSeasonId = Guid.NewGuid().ToString();

            var newSeason = await SeasonHelper.StartNewSeason(data.ClientToken, newSeasonId, conn);

            // Reset Elo numbers of all players for new season
            await PlayerHelper.StartNewSeason(data.ClientToken, newSeasonId, EloHelper.InitialEloNumber, conn);

            return new OkObjectResult(newSeason);
        }
    }
}
