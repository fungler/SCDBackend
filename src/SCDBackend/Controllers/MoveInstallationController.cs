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

        private CosmosConnector cc = CosmosConnector.Instance;


        [HttpPost("new")]
        public async Task<IActionResult> MoveInstallation([FromBody] InstallationRoot content) 
        {
            Subscription sub = await cc.GetSubscription(content.subscriptionId);
            Client client = await cc.GetClient("1");
            HttpResponseMessage SDDResponse = null;
            Installation i = null;

            try
            {
                //Adding random client since the JSON document doesn't contain a client
                i = new Installation(content.installation.name, "20.52.46.188:3389", sub, client, content.installation.state);
                await cc.CreateInstallationAsync(i);
            }
            catch (Exception)
            {
                return BadRequest("{\"status\": 500, \"message\": \"Error.\"}");
            }

            try
            {
                // Call endpoint from SDD and see if it went well
                SDDResponse = await WriteToSDD(content);
            }
            catch (Exception) when (!SDDResponse.IsSuccessStatusCode)
            {
                await cc.DeleteInstallation(i);
                return BadRequest("{\"status\": 500, \"message\": \"Error.\"}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await cc.DeleteInstallation(i);
                return BadRequest("{\"status\": 500, \"message\": \"Error.\"}");
            }

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
            HttpClient client = new HttpClient(clientHandler);

            HttpResponseMessage response = await client.GetAsync(sddBasePath + "/api/home/registerJson/getState?name="+instName);
         
            return response;
        }
    }
}