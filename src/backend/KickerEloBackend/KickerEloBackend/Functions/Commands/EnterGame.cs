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

namespace KickerEloBackend.Functions.Commands
{
    public static class EnterGame
    {
        [FunctionName("EnterGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<EnterGameCommand>(requestBody);

            using var conn = SqlHelper.GetSqlConnection();

            // Validate input data
            if (
                // Check if there are exactly two teams
                data.Teams.Count() != 2 ||
                // Check that each team has at least one player
                data.Teams.Any(team => !team.PlayerIDs.Any()) ||
                // Check that no player of team A is also part of team B
                data.Teams.Any(teamA => teamA.PlayerIDs.Any(id => data.Teams.Where(team => team.TeamNumber != teamA.TeamNumber).Any(teamB => teamB.PlayerIDs.Contains(id))))
                )
            {
                return new BadRequestObjectResult("The input data was invalid");
            }

            var newGameId = Guid.NewGuid().ToString();
            var newGame = await GameHelper.InsertGame(data.ClientToken, newGameId, conn);

            var result = await EloHelper.CalculateAndUpdateResults(conn, newGame, data);

            return new OkObjectResult(result);
        }
    }
}
