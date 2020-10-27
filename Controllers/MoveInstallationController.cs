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
                return BadRequest(SDDResponse.Content);
            }

            Subscription sub = await cc.GetSubScription(content.subscriptionId);
            //Console.WriteLine(sub);
            Client client = await cc.GetClient("1"); 

            // Adding random client since the JSON document doesn't contain a client
            Installation i = new Installation(content.installation.name, "20.52.46.188:3389", sub, client);

            HttpResponseMessage installationStateResponse = await GetState(i.name);

            if (installationStateResponse.IsSuccessStatusCode)
            {
                string installationState = await installationStateResponse.Content.ReadAsStringAsync();
                i.state = installationState;
            }
            else
            {
                i.state = "failed";
            }

            await cc.CreateInstallationAsync(i);

            // Respond to caller
            return Ok("{\"status\": 200, \"message\": \"Success.\"}");
        }

        private async Task<HttpResponseMessage> WriteToSDD(InstallationRoot instRoot) 
        {
            // bypass
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);

            var json = JsonSerializer.Serialize(instRoot);
            var response =  await client.PostAsync(sddBasePath + "/api/home/registerJson", new StringContent(json, Encoding.UTF8, "application/json"));
            return response;
        }

        private async Task<HttpResponseMessage> GetState(string instName)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);

            HttpResponseMessage response = await client.GetAsync(sddBasePath + "/api/home/registerJson/getState?name="+instName);
         
            return response;
        }
    }
}