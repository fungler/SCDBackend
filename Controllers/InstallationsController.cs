﻿using System;
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
                return BadRequest(e.StackTrace);
            }

        }
    }
}