using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using SCDBackend.Models;

namespace SCDBackend.Controllers
{
    public interface IPackageConnector
    {
        public Task<HttpResponseMessage> StartInstallation(string instName);

        public Task<HttpResponseMessage> StopInstallation(string instName);

        public Task<HttpResponseMessage> MoveInstallation(InstallationRoot content);

        public Task<HttpResponseMessage> GetStateAsync(string instName);

        public Task<HttpResponseMessage> CreateCopy(CopyData copy);

        public Task<HttpResponseMessage> GetInstallationDetails(string path);
    }
}