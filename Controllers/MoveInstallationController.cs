using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SCDBackend.DataAccess;
using SCDBackend.Models;
using System.Text.Json;
using System.Net.Http;
using System;
using System.Text;

namespace SCDBackend.Controllers
{
    [Route("api/new-inst")]
    [ApiController]


    public class MoveInstallationController : ControllerBase
    {
        private static string sddBasePath = "https://localhost:7001";
        private static CosmosConnector cc = new CosmosConnector();

        [HttpPost("new")]
        public async Task<IActionResult> MoveInstallation([FromBody] InstallationRoot content) 
        {
            // Call endpoint from SDD and see if it went well
            HttpResponseMessage SDDResponse = await WriteToSDD(content);

            if(!SDDResponse.IsSuccessStatusCode) 
            {
                Console.WriteLine("hello");
                return BadRequest(SDDResponse.Content);
            }

            Installation i = new Installation(content.installation.name, "20.52.46.188:3389", content.subscriptionId, null);
            await cc.CreateInstallationAsync(i);

            // Respond to caller
            return Ok("{\"status\": 200, \"message\": \"Success.\"}");
        }

        private async Task<HttpResponseMessage> WriteToSDD(InstallationRoot instRoot) 
        {
            HttpClient client = new HttpClient();
            var json = JsonSerializer.Serialize(instRoot);
            var response =  await client.PostAsync(sddBasePath + "/api/home/registerJson", new StringContent(json, Encoding.UTF8, "application/json"));
            return response;
        }
    }
}