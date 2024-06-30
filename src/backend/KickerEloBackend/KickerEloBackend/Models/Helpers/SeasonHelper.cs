using Dapper;
using KickerEloBackend.Models.DatabaseModels;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickerEloBackend.Models.Helpers
{
    internal class SeasonHelper
    {
        public async static Task<Season> GetCurrentSeason(string ClientToken, SqlConnection connection)
        {
            return await connection.QuerySingleAsync<Season>(@"SELECT SeasonID, SeasonID, SeasonNumber, StartDate, EndDate
            FROM seasons s
            INNER JOIN clients c ON s.ClientID=c.Id
            WHERE ClientToken=@ClientToken AND EndDate is null", new { ClientToken });
        }

        public async static Task<IEnumerable<Season>> GetSeasons(string ClientToken, SqlConnection connection)
        {
            return await connection.QueryAsync<Season>(@"SELECT SeasonID, SeasonID, SeasonNumber, StartDate, EndDate
            FROM seasons s
            INNER JOIN clients c ON s.ClientID=c.Id
            WHERE ClientToken=@ClientToken", new { ClientToken });
        }

        public static async Task<Season> StartNewSeason(string ClientToken, string newSeasonID, SqlConnection connection)
        {
            return await connection.QuerySingleAsync<Season>(@"
                DECLARE @PreviousSeasonNumber int = 0;
                UPDATE s
                SET EndDate = GETUTCDATE(), @PreviousSeasonNumber = s.SeasonNumber
                FROM seasons s
                INNER JOIN clients c ON s.ClientID=c.Id
                WHERE EndDate IS NULL AND ClientToken = @ClientToken

                INSERT INTO seasons (ClientID, SeasonID, SeasonNumber, StartDate)
                OUTPUT inserted.SeasonID, inserted.ClientID, inserted.SeasonNumber, inserted.StartDate, inserted.EndDate 
                SELECT Id, @newSeasonID, @PreviousSeasonNumber + 1, GETUTCDATE()
                FROM clients
                WHERE ClientToken=@ClientToken", new { ClientToken, newSeasonID });
        }
    }
}
