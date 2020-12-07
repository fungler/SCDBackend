using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SCDBackend.Models;
using System.Text.Json;
using System;
using System.Text;
using System.Collections.Generic;


namespace SCDBackend.Controllers
{
    public class PackageConnectorController : IPackageConnector
    {
        private static string PackageBasePath = "https://localhost:7001";

        // Task<String, Task<>>
        public async Task<HttpResponseMessage> GetStateAsync(string instName)
        {
            throw new System.NotImplementedException();
        }

        public async Task<HttpResponseMessage> MoveInstallation(InstallationRoot content)
        {
            throw new System.NotImplementedException();
        }

        public async Task<HttpResponseMessage> StartInstallation(string instName)
        {
            throw new System.NotImplementedException();
        }

        public async Task<HttpResponseMessage> StopInstallation(string instName)
        {
            throw new System.NotImplementedException();
        }
    }

}