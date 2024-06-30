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
using Dapper;

namespace KickerEloBackend.Functions.Queries
{
    public static class GetClient
    {
        [FunctionName("GetClient")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Client/{ClientToken}")] HttpRequest req, string ClientToken,
            ILogger log)
        {
            using var conn = SqlHelper.GetSqlConnection();
            var client = await ClientHelper.GetClient(ClientToken, conn);

            var numberOfPlayers = await conn.QuerySingleAsync<int>(@"SELECT COUNT(*) as NumberOfPlayers
            FROM players p
            INNER JOIN clients c ON p.ClientID=c.Id
            WHERE ClientToken=@ClientToken", new {ClientToken});

            var seasons = await SeasonHelper.GetSeasons(ClientToken, conn);

            var leader = await PlayerHelper.GetLeader(ClientToken, conn);

            var result = new ClientDetails()
            {
                ClientName = client.ClientName,
                CreationDate = client.CreationDate,
                NumberOfPlayers = numberOfPlayers,
                Seasons = seasons.Select(season => new SeasonResult(season)),
                CurrentLeader = leader
            };

            return new OkObjectResult(result);
        }
    }
}
