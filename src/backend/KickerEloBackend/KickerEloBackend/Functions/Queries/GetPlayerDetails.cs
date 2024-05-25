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
            var tableService = TablesHelper.GetTableServiceClient();
            var player = tableService.GetTableClient(DatabaseTables.PlayersTable).Query<Player>(x => x.PlayerID == PlayerID).First();

            var seasons = tableService.GetTableClient(DatabaseTables.SeasonsTable).Query<Season>(x => x.ClientID == player.ClientID);
            var currentSeason = seasons.First(s => s.EndDate == null);

            var playerElo = tableService.GetTableClient(DatabaseTables.PlayerEloTable).Query<PlayerElo>(x => x.PlayerID == PlayerID && x.SeasonID == currentSeason.SeasonID).First();

            var playerGameTable = tableService.GetTableClient(DatabaseTables.PlayerGameTable);

            var playerGameIds = playerGameTable.Query<PlayerGame>(x => x.PlayerID == player.PlayerID).Select(g => g.GameID);
            var games = tableService.GetTableClient(DatabaseTables.GamesTable).Query<Game>(g => g.ClientID == player.ClientID).Where(g => playerGameIds.Contains(g.GameID));
            var playerGames = playerGameTable.Query<PlayerGame>(x => x.ClientID == player.ClientID).Where(x => playerGameIds.Contains(x.GameID));

            log.LogInformation($"Found {games.Count()} games for this player");

            var result = new PlayerDetailsResult()
            {
                PlayerID = player.PlayerID,
                EloNumber = playerElo.EloNumber,
                LastUpdated = playerElo.LastUpdated,
                Nickname = player.Nickname,
                FullName = player.FullName,
                GamesPlayed = games.Select(g =>
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
                }).OrderByDescending(x => x.Date)
            };

            log.LogInformation($"Calculated result object");

            return new OkObjectResult(result);
        }
    }
}
