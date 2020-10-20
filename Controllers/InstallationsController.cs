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
        private static CosmosConnector cc = CosmosConnector.instance;

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

                string json =  await response.Content.ReadAsStringAsync();

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

            Console.WriteLine(data.client.name);

            try
            {
                InstallationCopy copyInstallation = new InstallationCopy(data.newName, "20.52.46.188:3389", data.Subscription, data.copyMethod, data.client);
                await cc.CreateInstallationAsync(copyInstallation);

                // skal serialize dataen vi får til et json object, så derfor har jeg bare lavet en ny class der kun har de felter som skal sendes videre til SDDBackend
                string jsonBody = JsonSerializer.Serialize(new CopyData(data.oldName, data.newName));

                HttpResponseMessage response = await client.PostAsync("https://localhost:7001/api/home/registerJson/copy", new StringContent(jsonBody, Encoding.UTF8, "application/json"));
                HttpStatusCode status = response.StatusCode;

                string json = await response.Content.ReadAsStringAsync();

                if (status == HttpStatusCode.OK)
                    return Ok(json);
                else{
                    Console.WriteLine("1");
                    return BadRequest(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("2" + e);
                return BadRequest(e.StackTrace);
            }
        }

        [HttpGet("subscriptions/all")]
        public async Task<IActionResult> getSubscriptions()
        {
            string json;
            try 
            {
                List<Subscription> sub = await cc.GetSubScriptions();
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
    }
}