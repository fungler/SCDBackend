using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCDBackend.DataAccess;
using SCDBackend.Models;
using System.Text.Json;

namespace SCDBackend.Controllers
{
    [Route("api/installations")]
    [ApiController]
    public class InstallationsController : ControllerBase
    {
        [HttpGet("all")]
        public async Task<IActionResult> getAllInstallations()
        {
            CosmosConnector cc = CosmosConnector.instance;
            await cc.establishConnection(); // ensure that we are connected to the db before use

            List<Installation> installations = await cc.GetInstallationsAsync();

            string json = JsonSerializer.Serialize(installations);

            return Ok(json);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getInstallation(string id)
        {
            CosmosConnector cc = CosmosConnector.instance;
            Installation inst = await cc.GetInstallationAsync(id);
            string json = JsonSerializer.Serialize(inst);
            return Ok(json);
        }
    }
}