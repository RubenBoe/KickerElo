using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Azure.Data.Tables;
using System;
using System.Web.Http;
using KickerEloBackend.Models;
using KickerEloBackend.Models.Helpers;

namespace KickerEloBackend.Functions.Commands
{
    public static class Status
    {
        [FunctionName("Status")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            using var conn = SqlHelper.GetSqlConnection();

            return new OkObjectResult("Service running");
        }
    }
}
