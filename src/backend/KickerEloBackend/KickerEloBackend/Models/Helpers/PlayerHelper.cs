using Dapper;
using KickerEloBackend.Models.DatabaseModels;
using KickerEloBackend.Models.Results;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickerEloBackend.Models.Helpers
{
    internal class PlayerHelper
    {
        /// <summary>
        /// Gets all players of the current seasons with their current elo
        /// </summary>
        /// <param name="ClientToken"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<PlayerResult>> GetPlayers(string ClientToken, SqlConnection connection)
        {
            return await connection.QueryAsync<PlayerResult>(@"SELECT p.PlayerID, p.Nickname, pe.EloNumber, pe.LastUpdated
                FROM players p
                INNER JOIN playerElo pe ON p.PlayerID=pe.PlayerID
                INNER JOIN seasons s ON pe.SeasonID=s.SeasonID
                INNER JOIN clients c ON c.Id=p.ClientID
                WHERE s.EndDate IS NULL AND c.ClientToken=@ClientToken
                ORDER BY EloNumber DESC", new { ClientToken });
        }

        public async static Task<PlayerResult> GetLeader(string ClientToken, SqlConnection connection)
        {
            return await connection.QuerySingleAsync<PlayerResult>(@"SELECT TOP 1 p.PlayerID, pe.EloNumber, LastUpdated, Nickname
            FROM players p
            INNER JOIN playerElo pe ON p.PlayerID=pe.PlayerID
			INNER JOIN seasons s ON pe.SeasonID=s.SeasonID
            INNER JOIN clients c ON p.ClientID=c.Id
            WHERE ClientToken=@ClientToken AND s.EndDate IS NULL
            ORDER BY pe.EloNumber DESC", new { ClientToken });
        }

        /// <summary>
        /// Gets the player details information of the current season
        /// </summary>
        /// <param name="PlayerID"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async static Task<PlayerDetailsResultBase> GetPlayerDetails(string PlayerID, SqlConnection connection)
        {
            var players = await connection.QuerySingleAsync<PlayerDetailsResultBase>(@"SELECT p.PlayerID, p.Nickname, p.FullName, pe.EloNumber, pe.LastUpdated
            FROM players p
            INNER JOIN playerElo pe ON pe.PlayerID=p.PlayerID
            INNER JOIN seasons eloseason ON pe.SeasonID=eloseason.SeasonID
            WHERE p.PlayerID = @PlayerID AND eloseason.EndDate IS NULL ", new { PlayerID });

            return players;
        }

        public async static Task<IEnumerable<Game>> GetGamesForPlayer(string PlayerID, SqlConnection connection)
        {
            return await connection.QueryAsync<Game>(@"SELECT g.GameID, g.SeasonID, g.ClientID, g.Date
                FROM  games g
                INNER JOIN playerGame pg2 ON pg2.GameID=g.GameID
                WHERE pg2.PlayerID = @PlayerID", new { PlayerID });
        }

        public async static Task<IEnumerable<PlayerGame>> GetPlayerGames(IEnumerable<string> GameIds, SqlConnection connection)
        {
            return await connection.QueryAsync<PlayerGame>(@"SELECT pg.GameID, pg.PlayerID, pg.Team, pg.Points, pg.EloGain
                FROM playerGame pg
                WHERE pg.GameID in @GameIds", new {GameIds});
        }

        public async static Task StartNewSeason(string ClientToken, string SeasonID, int initialElo, SqlConnection connection)
        {
            await connection.ExecuteAsync(@"
            INSERT INTO playerElo (PlayerID, SeasonID, LastUpdated, EloNumber)
                SELECT p.PlayerID, @SeasonID, GETUTCDATE(), @initialElo
                FROM players p
                INNER JOIN clients c ON p.ClientID=c.Id
                WHERE c.ClientToken=@ClientToken", new {ClientToken, SeasonID, initialElo});
        }

        public async static Task<Player> InsertNewPlayer(string ClientToken, string Nickname, string FullName, string PlayerID, int initialElo, SqlConnection connection)
        {
            return await connection.QuerySingleAsync<Player> (@"
            INSERT INTO players (ClientID, PlayerID, Nickname, FullName)
                OUTPUT inserted.ClientID, inserted.PlayerID, inserted.Nickname, inserted.FullName
                SELECT c.Id, @PlayerID, @Nickname, @FullName
                From clients c
                Where c.ClientToken=@ClientToken


            INSERT INTO playerElo (PlayerID, SeasonID, LastUpdated, EloNumber)
                SELECT @PlayerID, s.SeasonID, GETUTCDATE(), @initialElo
                FROM seasons s
                INNER JOIN clients c ON c.Id=s.ClientID
                WHERE c.ClientToken=@ClientToken AND s.EndDate IS NULL", new {ClientToken, Nickname, FullName, PlayerID, initialElo});
        }

        /// <summary>
        /// Saves the player game data and returns the new elo
        /// </summary>
        /// <param name="GameID"></param>
        /// <param name="PlayerID"></param>
        /// <param name="team"></param>
        /// <param name="points"></param>
        /// <param name="eloGain"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async static Task<int> InsertNewGame (string GameID, string PlayerID, int team, int points, int eloGain, SqlConnection connection)
        {
            return await connection.QuerySingleAsync<int>(@"
            INSERT INTO playerGame (GameID, PlayerID, Team, Points, EloGain)
                VALUES (@GameID, @PlayerID, @team, @points, @eloGain)

            UPDATE pe
                SET EloNumber = EloNumber + @eloGain, LastUpdated = GETUTCDATE()
                OUTPUT inserted.EloNumber
                FROM playerElo pe
                INNER JOIN seasons s ON pe.SeasonID=s.SeasonID
                WHERE pe.PlayerID=@PlayerID and s.EndDate IS NULL", new { GameID, PlayerID, team, points, eloGain } );
        }
    }
}
