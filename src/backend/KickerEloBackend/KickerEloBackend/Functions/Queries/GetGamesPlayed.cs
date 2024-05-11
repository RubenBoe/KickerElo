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
using KickerEloBackend.Models;
using KickerEloBackend.Models.DatabaseModels;
using KickerEloBackend.Models.Results;
using System.Numerics;
using System.Linq;

namespace KickerEloBackend.Functions.Queries
{
    public static class GetGamesPlayed
    {
        [FunctionName("GetGamesPlayed")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Games/{SeasonID}")] HttpRequest req, string SeasonID,
            ILogger log)
        {
            var tableService = TablesHelper.GetTableServiceClient();

            var season = tableService.GetTableClient(DatabaseTables.SeasonsTable).Query<Season>(x => x.SeasonID == SeasonID).First();

            var games = tableService.GetTableClient(DatabaseTables.GamesTable).Query<Game>(g => g.SeasonID == season.SeasonID);
            var playerGames = tableService.GetTableClient(DatabaseTables.PlayerGameTable).Query<PlayerGame>(x => x.ClientID == season.ClientID).Where(x => games.Select(x => x.GameID).Contains(x.GameID));

            var gamesPlayed = games.Select(g =>
                {
                    var correspondingPlayerGames = playerGames.Where(x => x.GameID == g.GameID);
                    var teams = correspondingPlayerGames.GroupBy(x => x.Team).Select(grouping =>
                    {
                        return new TeamGameResult()
                        {
                            TeamNumber = grouping.Key,
                            Points = grouping.First().Points,
                            PlayerResults = grouping.Select(pg => new PlayerGameResult()
                            {
                                EloGain = pg.EloGain,
                                PlayerID = pg.PlayerID,
                                EloNumber = -1
                            })
                        };
                    });
                    return new GameResult()
                    {
                        GameID = g.GameID,
                        SeasonID = g.SeasonID,
                        Date = g.Date,
                        TeamResults = teams
                    };
                }
            ).OrderByDescending(g => g.Date);

            return new OkObjectResult(gamesPlayed);
        }
    }
}
