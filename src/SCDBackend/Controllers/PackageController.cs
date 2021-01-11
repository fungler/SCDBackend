using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SCDBackend.Models;
using System.Text.Json;
using System;
using System.Text;
using System.Net;

namespace SCDBackend.Controllers
{
    public class PackageController : IPackageConnector
    {
        private static string PackageBasePath = "https://localhost:7001";

        private static HttpClientHandler ClientHandler;
        private static HttpClient Client;

        public PackageController()
        {
            ClientHandler = new HttpClientHandler();
            
            // Bypass certification
            ClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            Client = new HttpClient(ClientHandler);
        }

        public async Task<HttpResponseMessage> GetStateAsync(string instName)
        {
            HttpResponseMessage response = await Client.GetAsync(PackageBasePath + "/api/home/registerJson/getState?name=" + instName);
            return response;
        }

        public async Task<HttpResponseMessage> MoveInstallation(InstJsonDocument content)
        {
            var json = JsonSerializer.Serialize(content);
            var response = await Client.PostAsync(PackageBasePath + "/api/home/registerJson", new StringContent(json, Encoding.UTF8, "application/json"));
            return response;
        }

        public async Task<HttpResponseMessage> StartInstallation(string instName)
        {
            string json = "{\"name\": \"" + instName + "\"}";

            HttpResponseMessage res = await Client.PostAsync("https://localhost:7001/api/home/start", new StringContent(json, Encoding.UTF8, "application/json"));
            return res;
        }

        public async Task<HttpResponseMessage> StopInstallation(string instName)
        {
            string json = "{\"name\": \"" + instName + "\"}";

            HttpResponseMessage res = await Client.PostAsync("https://localhost:7001/api/home/stop", new StringContent(json, Encoding.UTF8, "application/json"));
            return res;
        }

        public async Task<HttpResponseMessage> CreateCopy(CopyData copy)
        {
            string json = JsonSerializer.Serialize(copy);
            return await Client.PostAsync("https://localhost:7001/api/home/registerJson/copy", new StringContent(json, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> GetInstallationDetails(string instName)
        {
            return await Client.GetAsync("https://localhost:7001/api/home/registerJson/getFile?instName=" + instName);
        }
    }
}