using Dapper;
using KickerEloBackend.Models.DatabaseModels;
using KickerEloBackend.Models.Results;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickerEloBackend.Models.Helpers
{
    internal class GameHelper
    {
        /// <summary>
        /// Calculates the GameResult object from the desired games
        /// </summary>
        /// <param name="games"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<GameResult>> GetGameResults (IEnumerable<Game> games, SqlConnection connection)
        {
            IEnumerable<PlayerGame> playerGames = await PlayerHelper.GetPlayerGames(games.Select(g => g.GameID), connection);

            var playerGamesLookup = playerGames.ToLookup(x => x.GameID);

            return games.Select(g =>
            {
                var correspondingPlayerGames = playerGamesLookup[g.GameID];
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
            }).OrderByDescending(x => x.Date);
        }

        public async static Task<IEnumerable<Game>> GetSeasonGames (string SeasonID, SqlConnection connection)
        {
            return await connection.QueryAsync<Game>(@"SELECT GameID, SeasonID, ClientID, Date
                FROM games
                WHERE SeasonID=@SeasonID", new { SeasonID });
        }

        public async static Task<Game> InsertGame (string ClientToken, string GameID, SqlConnection connection)
        {
            return await connection.QuerySingleAsync<Game>(@"
            INSERT INTO games (ClientID, GameID, SeasonID, Date) 
                OUTPUT inserted.ClientID, inserted.GameID, inserted.SeasonID, inserted.Date
                SELECT s.ClientID, @GameID, s.SeasonID, GETUTCDATE() 
                FROM seasons s
                INNER JOIN clients c ON c.Id = s.ClientID
                WHERE c.ClientToken=@ClientToken AND s.EndDate IS NULL", new { ClientToken, GameID });
        }
    }
}
