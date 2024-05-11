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
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<EnterGameCommand>(requestBody);

            var tableService = TablesHelper.GetTableServiceClient();
            var client = ClientHelper.GetClient(data.ClientToken, tableService);
            var currentSeason = tableService.GetTableClient(DatabaseTables.SeasonsTable).Query<Season>(s => s.EndDate == null && s.ClientID == client.Id).First();

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
            var newGame = new Game()
            {
                ClientID = client.Id,
                SeasonID = currentSeason.SeasonID,
                GameID = newGameId,
                RowKey = newGameId,
                Date = DateTime.UtcNow
            };
            await tableService.GetTableClient(DatabaseTables.GamesTable).AddEntityAsync(newGame);

            var result = await EloHelper.CalculateAndUpdateResults(tableService, newGame, data);

            return new OkObjectResult(result);
        }
    }
}
