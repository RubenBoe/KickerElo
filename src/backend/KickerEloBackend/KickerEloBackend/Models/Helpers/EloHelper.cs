using Dapper;
using KickerEloBackend.Models.CommandModels;
using KickerEloBackend.Models.DatabaseModels;
using KickerEloBackend.Models.Results;
using Microsoft.Data.SqlClient;
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

        public static async Task<GameResult> CalculateAndUpdateResults(SqlConnection conn, Game game, EnterGameCommand gameCommand)
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

            var winnerTeamElo = winnerTeam.PlayerIDs.Select(p => GetCurrentElo(conn, game.SeasonID, p)).Select(e => e.EloNumber).Average();
            var loserTeamElo = loserTeam.PlayerIDs.Select(p => GetCurrentElo(conn, game.SeasonID, p)).Select(e => e.EloNumber).Average();

            var newWinnerTeamElo = GetNewElo(winnerTeamElo, loserTeamElo, 1);
            var winnerTeamEloGain = (int)(newWinnerTeamElo - winnerTeamElo);

            var winnerTeamPlayerResults = new List<PlayerGameResult>();
            var loserTeamPlayerResults = new List<PlayerGameResult>();

            // Update player games and player elos
            foreach (var playerId in winnerTeam.PlayerIDs)
            {
                var newElo = await PlayerHelper.InsertNewGame(game.GameID, playerId, winnerTeam.TeamNumber, winnerTeam.Points, winnerTeamEloGain, conn);

                var resultPlayer = result.TeamResults.First(t => t.TeamNumber == winnerTeam.TeamNumber).PlayerResults.First(p => p.PlayerID == playerId);
                resultPlayer.EloGain = winnerTeamEloGain;
                resultPlayer.EloNumber = newElo;
                winnerTeamPlayerResults.Add(resultPlayer);
            }
            foreach (var playerId in loserTeam.PlayerIDs)
            {
                var newElo = await PlayerHelper.InsertNewGame(game.GameID, playerId, loserTeam.TeamNumber, loserTeam.Points, -winnerTeamEloGain, conn);

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

        public static PlayerElo GetCurrentElo (SqlConnection conn, string seasonId , string playerId)
        {
            // WARNING: NOT ASYNC!
            var playerElo = conn.QuerySingle<PlayerElo>("SELECT PlayerID, SeasonID, EloNumber, LastUpdated FROM playerElo WHERE SeasonID=@seasonId AND PlayerID=@playerId", new {seasonId, playerId});
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
            var expectation = 1 / (1 + Math.Pow(10, (opponentElo - previousElo) / divisor));
            var newElo = previousElo + kNumber * (outcome - expectation);

            return (int)newElo;
        }
    }
}
