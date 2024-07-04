using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KickerEloBackend.Models.Helpers;
using System.Text.Json;
using KickerEloBackend.Models.CommandModels;

namespace KickerEloBackend.Functions.Commands
{
    public static class RevertLastGame
    {
        [FunctionName("RevertLastGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<RevertLastGameCommand>(requestBody);

            using var conn = SqlHelper.GetSqlConnection();

            await GameHelper.RevertLastGame(data.ClientToken, conn);

            return new OkResult();
        }
    }
}
