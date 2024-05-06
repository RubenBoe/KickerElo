using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using KickerEloBackend.Models;

namespace KickerEloBackend
{
    public static class CreateClient
    {
        [FunctionName("CreateClient")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var client = JsonSerializer.Deserialize<Client>(requestBody);

            string responseMessage = $"Created {client.ClientName} with ID {client.Id}.";

            return new OkObjectResult(responseMessage);
        }
    }
}
