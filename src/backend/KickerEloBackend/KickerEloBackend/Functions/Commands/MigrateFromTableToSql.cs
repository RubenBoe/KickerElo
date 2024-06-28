using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KickerEloBackend.Models.Helpers;
using KickerEloBackend.Models.DatabaseModels;
using KickerEloBackend.Models;
using Dapper;

namespace KickerEloBackend.Functions.Commands
{
    public static class MigrateFromTableToSql
    {
        [FunctionName("MigrateFromTableToSql")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var tableService = TablesHelper.GetTableServiceClient();
            var clients = tableService.GetTableClient(DatabaseTables.ClientsTable).Query<Client>();
            var seasons = tableService.GetTableClient(DatabaseTables.SeasonsTable).Query<Season>();
            var players = tableService.GetTableClient(DatabaseTables.PlayersTable).Query<Player>();
            var playerElos = tableService.GetTableClient(DatabaseTables.PlayerEloTable).Query<PlayerElo>();
            var games = tableService.GetTableClient(DatabaseTables.GamesTable).Query<Game>();
            var playerGames = tableService.GetTableClient(DatabaseTables.PlayerGameTable).Query<PlayerGame>();

            using var conn = SqlHelper.GetSqlConnection();

            await conn.ExecuteAsync("SET IDENTITY_INSERT dbo.clients ON");
            foreach (var client in clients)
            {
                await conn.ExecuteAsync("INSERT INTO dbo.clients (Id, ClientName, CreationDate, ClientToken) VALUES (@Id, @ClientName, @CreationDate, @ClientToken)", client);
            }
            await conn.ExecuteAsync("SET IDENTITY_INSERT dbo.clients OFF");

            foreach (var season in seasons)
            {
                await conn.ExecuteAsync("INSERT INTO dbo.seasons (SeasonID, ClientID, SeasonNumber, StartDate, EndDate) VALUES (@SeasonID, @ClientID, @SeasonNumber, @StartDate, @EndDate)", season);
            }
            foreach (var player in players)
            {
                await conn.ExecuteAsync("INSERT INTO dbo.players (PlayerID, ClientID, Nickname, FullName) VALUES (@PlayerID, @ClientID, @Nickname, @FullName)", player);
            }
            foreach (var playerElo in playerElos)
            {
                await conn.ExecuteAsync("INSERT INTO dbo.playerElo (PlayerID, SeasonID, EloNumber, LastUpdated) VALUES (@PlayerID, @SeasonID, @EloNumber, @LastUpdated)", playerElo);
            }
            foreach (var game in games)
            {
                await conn.ExecuteAsync("INSERT INTO dbo.games (GameID, SeasonID, ClientID, Date) VALUES (@GameID, @SeasonID, @ClientID, @Date)", game);
            }
            foreach (var playerGame in playerGames)
            {
                await conn.ExecuteAsync("INSERT INTO dbo.playerGame (GameID, PlayerID, Team, Points, EloGain) SELECT @GameID, @PlayerID, @Team, @Points, @EloGain WHERE EXISTS (SELECT 1 FROM dbo.games WHERE GameID = @GameID)", playerGame);
            }

            return new OkObjectResult("");
        }
    }
}
