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
using KickerEloBackend.Models.Results;
using KickerEloBackend.Models;
using KickerEloBackend.Models.DatabaseModels;
using System.Linq;

namespace KickerEloBackend.Functions.Queries
{
    public static class GetClient
    {
        [FunctionName("GetClient")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Client/{ClientToken}")] HttpRequest req, string ClientToken,
            ILogger log)
        {
            var tableService = TablesHelper.GetTableServiceClient();
            var client = ClientHelper.GetClient(ClientToken, tableService);

            var playersTable = tableService.GetTableClient(DatabaseTables.PlayersTable);
            var numberOfPlayers = playersTable.Query<Player>(x => x.ClientID == client.Id).Count();

            var seasons = tableService.GetTableClient(DatabaseTables.SeasonsTable).Query<Season>(x => x.ClientID == client.Id).OrderByDescending(x => x.StartDate);
            var currentSeason = seasons.First(x => x.EndDate == null);

            var topEloPlayer = tableService.GetTableClient(DatabaseTables.PlayerEloTable).Query<PlayerElo>(x => x.SeasonID == currentSeason.SeasonID).OrderByDescending(x => x.EloNumber).First();
            var topEloPlayerInfo = playersTable.Query<Player>(x => x.PlayerID == topEloPlayer.PlayerID && x.ClientID == client.Id).First();

            var result = new ClientDetails()
            {
                ClientName = client.ClientName,
                CreationDate = client.CreationDate,
                NumberOfPlayers = numberOfPlayers,
                Seasons = seasons.Select(season => new SeasonResult(season)),
                CurrentLeader = new PlayerResult()
                {
                    PlayerID = topEloPlayer.PlayerID,
                    EloNumber = topEloPlayer.EloNumber,
                    LastUpdated = topEloPlayer.LastUpdated,
                    Nickname = topEloPlayerInfo.Nickname
                }
            };

            return new OkObjectResult(result);
        }
    }
}
