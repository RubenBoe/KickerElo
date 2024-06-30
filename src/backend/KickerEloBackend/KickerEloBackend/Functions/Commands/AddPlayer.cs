using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using KickerEloBackend.Models.CommandModels;
using KickerEloBackend.Models.Helpers;
using KickerEloBackend.Models.DatabaseModels;
using KickerEloBackend.Models;
using System.Linq;
using System.Web.Http;

namespace KickerEloBackend.Functions.Commands
{
    public static class AddPlayer
    {
        [FunctionName("AddPlayer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonSerializer.Deserialize<AddPlayerCommand>(requestBody);

                using var conn = SqlHelper.GetSqlConnection();

                var newPlayerId = Guid.NewGuid().ToString();

                var newPlayer = await PlayerHelper.InsertNewPlayer(data.ClientToken, data.Nickname, data.FullName, newPlayerId, EloHelper.InitialEloNumber, conn);

                return new OkObjectResult(newPlayer);
            } catch (Exception e)
            {
                log.LogError(e.Message, e);
                return new InternalServerErrorResult();
            }
        }
    }
}
