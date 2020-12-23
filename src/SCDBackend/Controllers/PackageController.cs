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
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient();
            string json = "{'name': '" + instName + "'}";

            HttpResponseMessage res = await client.PostAsync("https://localhost:7001/api/home/start", new StringContent(json, Encoding.UTF8, "application/json"));
            return res;
        }

        public async Task<HttpResponseMessage> StopInstallation(string instName)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient();
            string json = "{'name': '" + instName + "'}";

            HttpResponseMessage res = await client.PostAsync("https://localhost:7001/api/home/stop", new StringContent(json, Encoding.UTF8, "application/json"));
            return res;
        }
    }
}