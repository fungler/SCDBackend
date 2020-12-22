using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCDBackend.DataAccess;
using SCDBackend.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Runtime.InteropServices.ComTypes;

namespace SCDBackend.Controllers
{
    [Route("api/installations")]
    [ApiController]
    public class InstallationsController : ControllerBase
    {
        private static CosmosConnnectorCreator devPreset = new CosmosConnnectorCreator(Db.Dev);
        private static CosmosConnector cc = new CosmosConnector(devPreset);

        private static CosmosConnnectorCreator testPreset = new CosmosConnnectorCreator(Db.Test);
        private static CosmosConnector tcc = new CosmosConnector(testPreset);

        [HttpGet("all")]
        public async Task<IActionResult> getAllInstallations([FromQuery] bool isTest = false)
        {
            string json;
            try
            {
                List<Installation> installations;
                if (isTest)
                    installations = await tcc.GetInstallationsAsync();
                else
                    installations = await cc.GetInstallationsAsync();

                json = JsonSerializer.Serialize(installations);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }

            return Ok(json);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> getInstallation(string name, [FromQuery] bool isTest = false)
        {
            string json;
            try
            {
                Installation inst;
                if (isTest)
                    inst = await tcc.GetInstallationAsync(name);
                else
                    inst = await cc.GetInstallationAsync(name);

                json = JsonSerializer.Serialize(inst);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }

            return Ok(json);
        }

        [HttpGet("json")]
        public async Task<IActionResult> getInstallationJson([FromQuery] string path)
        {

            // Der er problemer med ssl certification, så dette er bare en måde at bypass'e det
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);

            try
            {
                HttpResponseMessage response = await client.GetAsync("https://localhost:7001/api/home/registerJson/getFile?path=" + path);

                string json = await response.Content.ReadAsStringAsync();

                return Ok(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.StackTrace);
            }

        }


        [HttpPost("json/copy")]
        public async Task<IActionResult> createInstallationCopy([FromBody] CopyDataDB data)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);

            try
            {
                InstallationCopy copyInstallation = new InstallationCopy(data.newName, "20.52.46.188:3389", data.Subscription, data.copyMethod, data.client);

                HttpResponseMessage installationStateResponse = await GetState(data.oldName);

                if (installationStateResponse.IsSuccessStatusCode)
                {
                    string installationState = await installationStateResponse.Content.ReadAsStringAsync();
                    copyInstallation.state = installationState;
                }
                else
                {
                    copyInstallation.state = "failed";
                }

                await cc.CreateInstallationAsync(copyInstallation);

                string jsonBody = JsonSerializer.Serialize(new CopyData(data.oldName, data.newName));

                HttpResponseMessage response = await client.PostAsync("https://localhost:7001/api/home/registerJson/copy", new StringContent(jsonBody, Encoding.UTF8, "application/json"));
                HttpStatusCode status = response.StatusCode;

                string json = await response.Content.ReadAsStringAsync();

                if (status == HttpStatusCode.OK)
                    return Ok(json);
                else {
                    return BadRequest(json);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }
        }

        [HttpGet("subscriptions/all")]
        public async Task<IActionResult> getSubscriptions([FromQuery] bool isTest = false)
        {
            string json;
            try
            {
                List<Subscription> sub;
                if (isTest)
                    sub = await tcc.GetSubscriptions();
                else
                    sub = await cc.GetSubscriptions();

                json = JsonSerializer.Serialize(sub);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }

            return Ok(json);
        }

        [HttpGet("clients/all")]
        public async Task<IActionResult> getClients([FromQuery] bool isTest = false)
        {
            string json;
            try
            {
                List<Client> clients;
                if (isTest)
                    clients = await tcc.GetClients();
                else
                    clients = await cc.GetClients();

                json = JsonSerializer.Serialize(clients);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }
            return Ok(json);
        }
        
        [HttpGet("item/getId")]
        public async Task<IActionResult> getItemId([FromQuery] string name, [FromQuery] bool isTest = false)
        {
            string json;
            try
            {
                if (isTest)
                    json = await tcc.GetItemId(name);
                else
                    json = await cc.GetItemId(name);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }
            return Ok(json);
        }

        [HttpGet("start")]
        public async Task<IActionResult> startInstallation([FromQuery] string name, [FromQuery] bool isTest = false)
        {
            try
            {
                int status;
                if (isTest)
                    status = await tcc.StartInstallation(name);
                else
                    status = await cc.StartInstallation(name);

                if (status == 1)
                    return Ok("{\"status\": 200, \"message\": \"Success.\"}");
                else
                    return BadRequest("{\"status\": 400, \"message\": \"Failed to find installation - check installation name.\"}");
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }
        }

        [HttpGet("stop")]
        public async Task<IActionResult> stopInstallation([FromQuery] string name, [FromQuery] bool isTest = false)
        {
            try
            {
                int status;
                if (isTest)
                    status = await tcc.StopInstallation(name);
                else
                    status = await cc.StopInstallation(name);

                if (status == 1)
                    return Ok("{\"status\": 200, \"message\": \"Success.\"}");
                else
                    return BadRequest("{\"status\": 400, \"message\": \"Failed to find installation - check installation name.\"}");
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }
        }

        private async Task<HttpResponseMessage> GetState(string instName)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);

            HttpResponseMessage response = await client.GetAsync("https://localhost:7001/api/home/registerJson/getState?name=" + instName);

            return response;
        }
    }
}