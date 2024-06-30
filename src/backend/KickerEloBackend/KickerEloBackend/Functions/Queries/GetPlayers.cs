using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KickerEloBackend.Models.Helpers;
using System.Web.Http;

namespace KickerEloBackend.Functions.Queries
{
    public static class GetPlayers
    {
        [FunctionName("GetPlayers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Players/{ClientToken}")] HttpRequest req, string ClientToken,
            ILogger log)
        {
            try
            {
                using var conn = SqlHelper.GetSqlConnection();

                var result = await PlayerHelper.GetPlayers(ClientToken, conn);

                log.LogInformation("created result", result);

                return new OkObjectResult(result);
            } catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                return new InternalServerErrorResult();
            }
        }
    }
}
