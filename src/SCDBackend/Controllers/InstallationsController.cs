using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SCDBackend.DataAccess;
using SCDBackend.Models;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Net;

namespace SCDBackend.Controllers
{
    [Route("api/installations")]
    [ApiController]
    public class InstallationsController : ControllerBase
    {
        private CosmosConnector cc = CosmosConnector.Instance;
        private PackageConnectorController pc = new PackageConnectorController();

        private HttpClient client = new HttpClient();

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
            try
            {
                var response = await pc.GetInstallationDetails(path);
                
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

            // Write to SDDBackend
            var packageCopy = new CopyData(data.oldName, data.newName);

            try 
            {
                var res = await pc.CreateCopy(packageCopy);
            }
            catch (HttpRequestException e) when (!e.Message.Contains("200"))
            {
                return BadRequest();
            }

            // Getting data ready for database
            var dbCopy = new InstallationCopy(data.newName, "20.52.46.188:3389", data.Subscription, data.copyMethod, data.client);
            
            // Check state of the installation that has been copied
            try 
            {
                var instStateRes = await pc.GetStateAsync(data.oldName);
            }
            catch (HttpRequestException e) when (!e.Message.Contains("200"))
            {
                dbCopy.state = "failed";
            }

            // Write to database
            try
            {
                await cc.CreateInstallationAsync(dbCopy); // TODO: Check if failed
            }
            catch (Exception e)
            {
                // TODO: Delete copy from SDD
                return BadRequest();
            }

            return Ok();
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

        [HttpPost("new")]
        public async Task<IActionResult> MoveInstallation([FromBody] InstallationRoot content)
        {
            var subscription = await cc.GetSubscription(content.subscriptionId);
            var client = await cc.GetClient("1"); // Client is not part of the Json document
            HttpResponseMessage SDDResponse = null;

            var installation = new Installation(content.installation.name, "20.52.46.188:3389", subscription, client, content.installation.state);

            try
            {
                await cc.CreateInstallationAsync(installation);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            try
            {
                SDDResponse = await pc.MoveInstallation(content);
            }
            catch (Exception) when (!SDDResponse.IsSuccessStatusCode)
            {
                await cc.DeleteInstallation(installation);
                return BadRequest("{\"status\": 500, \"message\": \"Error.\"}");
            }
            
            string body = SDDResponse.Content.ReadAsStringAsync().Result;
            SDDResponse sddres = Newtonsoft.Json.JsonConvert.DeserializeObject<SDDResponse>(body);

            return Ok("{\"status\": 200, \"message\": \"Success.\", \"installation_status\": \"" + sddres.installation_status + "\"}");

        }
    }
}