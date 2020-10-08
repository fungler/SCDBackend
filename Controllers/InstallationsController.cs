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

namespace SCDBackend.Controllers
{
    [Route("api/installations")]
    [ApiController]
    public class InstallationsController : ControllerBase
    {
        [HttpGet("all")]
        public async Task<IActionResult> getAllInstallations()
        {
            string json;

            try
            {
                CosmosConnector cc = CosmosConnector.instance;
                await cc.establishConnection(); // ensure that we are connected to the db before use

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
                CosmosConnector cc = CosmosConnector.instance;
                await cc.establishConnection();

                Installation inst = await cc.GetInstallationAsync(name);

                json = JsonSerializer.Serialize(inst);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
            }

            return Ok(json);
        }
    }
}