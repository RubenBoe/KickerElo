using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KickerEloBackend.Models.CommandModels;
using KickerEloBackend.Models.DatabaseModels;
using KickerEloBackend.Models.Helpers;
using KickerEloBackend.Models;
using System.Text.Json;
using System.Linq;
using System;
using System.Web.Http;

namespace KickerEloBackend.Functions.Commands
{
    public static class CalculateExpectedOutcome
    {
        [FunctionName("CalculateExpectedOutcome")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try { 
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonSerializer.Deserialize<CalculateExpectedOutcomeCommand>(requestBody);

                var tableService = TablesHelper.GetTableServiceClient();
                var client = ClientHelper.GetClient(data.ClientToken, tableService);
                var seasons = tableService.GetTableClient(DatabaseTables.SeasonsTable).Query<Season>(x => x.ClientID == client.Id);
                var currentSeason = seasons.First(s => s.EndDate == null);

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

                var responseMessage = EloHelper.SimulateGame(tableService, data.Teams, currentSeason.SeasonID);

                return new OkObjectResult(responseMessage);
            } catch(Exception e)
            {
                log.LogError($"Error as {e.Message}, {e.StackTrace}", e);
                return new InternalServerErrorResult();
            }
        }
    }
}
