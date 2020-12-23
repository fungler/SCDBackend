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
        private CosmosConnector cc = CosmosConnector.Instance;
        private PackageConnectorController pc = new PackageConnectorController();

        [HttpGet("all")]
        public async Task<IActionResult> getAllInstallations()
        {
            string json;
            try
            {
                List<Installation> installations = await cc.GetInstallationsAsync();
                json = JsonSerializer.Serialize(installations);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }

            return Ok(json);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> getInstallation(string name)
        {
            string json;
            try
            {
                Installation inst = await cc.GetInstallationAsync(name);
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

                HttpResponseMessage installationStateResponse = await pc.GetStateAsync(data.oldName);

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
        public async Task<IActionResult> getSubscriptions()
        {
            string json;
            try
            {
                List<Subscription> sub = await cc.GetSubscriptions();
                json = JsonSerializer.Serialize(sub);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }

            return Ok(json);
        }

        [HttpGet("clients/all")]
        public async Task<IActionResult> getClients()
        {
            string json;
            try
            {
                List<Client> clients = await cc.GetClients();
                json = JsonSerializer.Serialize(clients);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }
            return Ok(json);
        }
        
        [HttpGet("item/getId")]
        public async Task<IActionResult> getItemId([FromQuery] string name)
        {
            string json;
            try
            {
                json = await cc.GetItemId(name);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }
            return Ok(json);
        }

        [HttpGet("start")]
        public async Task<IActionResult> startInstallation([FromQuery] string name)
        {
            try
            {
                HttpResponseMessage res = await pc.StartInstallation(name);
                string body = res.Content.ReadAsStringAsync().Result;
                SDDResponse sddres = Newtonsoft.Json.JsonConvert.DeserializeObject<SDDResponse>(body);

                if (res.IsSuccessStatusCode)
                {
                    bool success = await cc.StartInstallation(name);

                    if (success)
                        return Ok("{\"status\": 200, \"message\": \"Success.\", \"installation_status\": \"" + sddres.installation_status +"\"}");
                    else
                        return BadRequest("{\"status\": 400, \"message\": \"Failed to find installation.\", \"installation_status\": \"STATUS_START_FAILED\"}");
                }
                else
                    return BadRequest("{\"status\": 400, \"message\": \"Failed to start installation.\", \"installation_status\": \"" + sddres.installation_status + "\"}");
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }
        }

        [HttpGet("stop")]
        public async Task<IActionResult> stopInstallation([FromQuery] string name)
        {
            try
            {
                HttpResponseMessage res = await pc.StopInstallation(name);
                string body = res.Content.ReadAsStringAsync().Result;
                SDDResponse sddres = Newtonsoft.Json.JsonConvert.DeserializeObject<SDDResponse>(body);

                if (res.IsSuccessStatusCode)
                {
                    bool success = await cc.StopInstallation(name);

                    if (success)
                        return Ok("{\"status\": 200, \"message\": \"Success.\", \"installation_status\": \"" + sddres.installation_status + "\"}");
                    else
                        return BadRequest("{\"status\": 400, \"message\": \"Failed to find installation.\", \"installation_status\": \"STATUS_STOP_FAILED\"}");
                }
                else
                    return BadRequest("{\"status\": 400, \"message\": \"Failed to stop installation.\", \"installation_status\": \"" + sddres.installation_status + "\"}");
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }
        }

    }
}