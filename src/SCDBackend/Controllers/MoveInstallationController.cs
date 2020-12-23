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
        private CosmosConnector cc = CosmosConnector.Instance;
        private PackageConnectorController pc = new PackageConnectorController();


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
                SDDResponse = await pc.MoveInstallation(content);
            }
            catch (Exception) when (!SDDResponse.IsSuccessStatusCode)
            {
                await cc.DeleteInstallation(i);
                return BadRequest("{\"status\": 500, \"message\": \"Error.\"}");
            }
            catch (Exception)
            {
                await cc.DeleteInstallation(i);
                return BadRequest("{\"status\": 500, \"message\": \"Error.\"}");
            }

            string body = SDDResponse.Content.ReadAsStringAsync().Result;
            SDDResponse sddres = Newtonsoft.Json.JsonConvert.DeserializeObject<SDDResponse>(body);

            return Ok("{\"status\": 200, \"message\": \"Success.\", \"installation_status\": \"" + sddres.installation_status + "\"}");
        }

    }
}