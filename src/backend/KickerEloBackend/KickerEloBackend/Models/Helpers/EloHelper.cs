

using Azure;
using Azure.Data.Tables;
using KickerEloBackend.Models.CommandModels;
using KickerEloBackend.Models.DatabaseModels;
using KickerEloBackend.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KickerEloBackend.Models.Helpers
{
    internal static class EloHelper
    {
        public const int InitialEloNumber = 1200;
        // This constant is used for the calculation of the new elo number
        private const int kNumber = 15;
        // Number used to divide by in the calculation of the expectation value. This is a random number. In the chess system, 400 is chosen
        private const int divisor = 400;

        public static async Task<GameResult> CalculateAndUpdateResults(TableServiceClient tableService, Game game, EnterGameCommand gameCommand)
        {
            var result = new GameResult()
            {
                Date = game.Date,
                SeasonID = game.SeasonID,
                GameID = game.GameID,
                TeamResults = gameCommand.Teams.Select(team => new TeamGameResult()
                {
                    TeamNumber = team.TeamNumber,
                    Points = team.Points,
                    PlayerResults = team.PlayerIDs.Select(p => new PlayerGameResult() { PlayerID = p })
                }),
            };

            var winnerTeam = gameCommand.Teams.OrderBy(t => t.Points).Last();
            var loserTeam = gameCommand.Teams.OrderBy(t => t.Points).First();

            var winnerTeamElo = winnerTeam.PlayerIDs.Select(p => GetCurrentElo(tableService, game.SeasonID, p)).Select(e => e.EloNumber).Average();
            var loserTeamElo = loserTeam.PlayerIDs.Select(p => GetCurrentElo(tableService, game.SeasonID, p)).Select(e => e.EloNumber).Average();

            var newWinnerTeamElo = GetNewElo(winnerTeamElo, loserTeamElo, 1);
            var winnerTeamEloGain = (int)(newWinnerTeamElo - winnerTeamElo);

            var eloTable = tableService.GetTableClient(DatabaseTables.PlayerEloTable);

            var winnerTeamPlayerResults = new List<PlayerGameResult>();
            var loserTeamPlayerResults = new List<PlayerGameResult>();
            // Update player games and player elos
            foreach (var playerId in winnerTeam.PlayerIDs)
            {
                await tableService.GetTableClient(DatabaseTables.PlayerGameTable).AddEntityAsync(new PlayerGame()
                {
                    EloGain = winnerTeamEloGain,
                    GameID = game.GameID,
                    PlayerID = playerId,
                    Points = winnerTeam.Points,
                    Team = winnerTeam.TeamNumber,
                    RowKey = $"{game.GameID}_{playerId}",
                    ClientID = game.ClientID
                });
                var currentElo = GetCurrentElo(tableService, game.SeasonID, playerId);
                var newElo = currentElo.EloNumber + winnerTeamEloGain;
                await eloTable.UpdateEntityAsync(new PlayerElo(playerId, game.SeasonID, newElo), ETag.All);

                var resultPlayer = result.TeamResults.First(t => t.TeamNumber == winnerTeam.TeamNumber).PlayerResults.First(p => p.PlayerID == playerId);
                resultPlayer.EloGain = winnerTeamEloGain;
                resultPlayer.EloNumber = newElo;
                winnerTeamPlayerResults.Add(resultPlayer);
            }
            foreach (var playerId in loserTeam.PlayerIDs)
            {
                await tableService.GetTableClient(DatabaseTables.PlayerGameTable).AddEntityAsync(new PlayerGame()
                {
                    EloGain = -winnerTeamEloGain,
                    GameID = game.GameID,
                    PlayerID = playerId,
                    Points = loserTeam.Points,
                    Team = loserTeam.TeamNumber,
                    RowKey = $"{game.GameID}_{playerId}",
                    ClientID = game.ClientID
                });
                var currentElo = GetCurrentElo(tableService, game.SeasonID, playerId);
                var newElo = currentElo.EloNumber - winnerTeamEloGain;
                await eloTable.UpdateEntityAsync(new PlayerElo(playerId, game.SeasonID, newElo), ETag.All);

                var resultPlayer = result.TeamResults.First(t => t.TeamNumber == loserTeam.TeamNumber).PlayerResults.First(p => p.PlayerID == playerId);
                resultPlayer.EloGain = -winnerTeamEloGain;
                resultPlayer.EloNumber = newElo;
                loserTeamPlayerResults.Add(resultPlayer);
            }

            result.TeamResults = result.TeamResults.Select(
                teamResult =>
                {
                    if (teamResult.TeamNumber == loserTeam.TeamNumber)
                    {
                        teamResult.PlayerResults = loserTeamPlayerResults;
                    }
                    else
                    {
                        teamResult.PlayerResults = winnerTeamPlayerResults;
                    }
                    return teamResult;
                }
            );

            return result;
        }

        public static ExpectedOutcomeResult SimulateGame (TableServiceClient tableServiceClient, IEnumerable<TeamBase> teams, string currentSeasonID)
        {
            var teamElos = teams.ToDictionary(
                t => t.TeamNumber,
                t =>
                {
                    var playerElos = t.PlayerIDs.Select(p => GetCurrentElo(tableServiceClient, currentSeasonID, p).EloNumber);
                    return playerElos.Average();
                }
                );

            var firstTeamExpectationValue = GetExpectationValue(teamElos[1], teamElos[2]);

            var firstTeamScore = 0;
            var opponentScore = 0;
            
            Random rnd = new Random();
            while (firstTeamScore < 10 && opponentScore < 10)
            {
                var diceRoll = rnd.NextDouble();

                if(diceRoll < firstTeamExpectationValue)
                {
                    firstTeamScore++;
                } else
                {
                    opponentScore++;
                }
            }

            return new ExpectedOutcomeResult()
            {
                Teams = teams.Select(t => new Team()
                {
                    PlayerIDs = t.PlayerIDs,
                    Points = t.TeamNumber == 1 ? firstTeamScore : opponentScore,
                    TeamNumber = t.TeamNumber,
                })
            };
        }

        public static PlayerElo GetCurrentElo (TableServiceClient tableService, string seasonId , string playerId)
        {
            var playerElo = tableService.GetTableClient(DatabaseTables.PlayerEloTable).Query<PlayerElo>(pe => pe.PlayerID == playerId && pe.SeasonID == seasonId).First();
            return playerElo;
        }

        /// <summary>
        /// Calculates the new elo number.
        /// </summary>
        /// <param name="previousElo">Elo number before the match</param>
        /// <param name="outcome">0 if the player lost, 1 if the player won</param>
        /// <returns>The new elo number</returns>
        public static int GetNewElo (double previousElo, double opponentElo, int outcome)
        {
            var expectation = GetExpectationValue(previousElo, opponentElo);
            var newElo = previousElo + kNumber * (outcome - expectation);

            return (int)newElo;
        }

        public static double GetExpectationValue (double currentElo, double opponentElo) 
        {
            return 1 / (1 + Math.Pow(10, (opponentElo - currentElo) / divisor));
        }
    }
}
