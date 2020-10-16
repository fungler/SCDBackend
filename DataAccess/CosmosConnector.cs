using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using SCDBackend.Models;

namespace SCDBackend.DataAccess
{
    public sealed class CosmosConnector
    {

        public static CosmosConnector instance { get; } = new CosmosConnector();

        private static readonly string EndpointUri = "https://fungler.documents.azure.com:443/";
        private static readonly string PrimaryKey = "Gs9XLbKvsstuGzfUpbCYNufDBER0o9Ony3WmBo8drTMp4ugd48s2xqAiKQI5Ve9yXiBFnqXqu3Sj8T607uouPA==";

        private static string databaseId = "frontend_test";
        private static string containerId = "dummyInstallations";

        private static CosmosClient cosmosClient;
        private static Microsoft.Azure.Cosmos.Database database;
        private static Container container;

        public CosmosConnector() {}

        private static async Task InitAsync()
        {
            if (cosmosClient == null || database == null || container == null)
            {
                cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
                database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
                container = await database.CreateContainerIfNotExistsAsync(new ContainerProperties(containerId, "/installation"));
            }
        }

        private async Task EstablishConnection()
        {
            try
            {
                await InitAsync();
            }
            catch (CosmosException e)
            {
                Console.WriteLine("Cosmos except" + e.Data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Except" + e.Data);
            }
        }

        public async Task<List<Installation>> GetInstallationsAsync()
        {
            await EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c");

            FeedIterator<Installation> queryResultSetIterator = container.GetItemQueryIterator<Installation>(qd);
            List<Installation> res = new List<Installation>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Installation> currentResultSet = await queryResultSetIterator.ReadNextAsync();

                foreach (Installation installation in currentResultSet)
                {
                    res.Add(installation);
                }
            }

            return res;
        }

        public async Task<Installation> GetInstallationAsync(string name)
        {
            await EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c WHERE c.name = @name")
                .WithParameter("@name", name);

            FeedIterator<Installation> queryResultSetIterator = container.GetItemQueryIterator<Installation>(qd);
            Installation inst = null;

            if (queryResultSetIterator.HasMoreResults)
            {
                Console.Write("testing something");
                FeedResponse<Installation> res = await queryResultSetIterator.ReadNextAsync();
                foreach (Installation i in res)
                {
                    inst = i;
                }
            }
            Console.Write(inst);
            return inst;
        }

        // TODO check if installation exists
        public async Task CreateInstallationAsync(Installation installation) 
        {
            await EstablishConnection();
            Container c = cosmosClient.GetDatabase(databaseId).GetContainer(containerId);
            var installationItemResponse = await c.CreateItemAsync<Installation>(installation, new PartitionKey(installation.installation));
        }
        // overload for installation copy
        public async Task CreateInstallationAsync(InstallationCopy installation)
        {
            await EstablishConnection();
            Container c = cosmosClient.GetDatabase(databaseId).GetContainer(containerId);
            var installationItemResponse = await c.CreateItemAsync<InstallationCopy>(installation, new PartitionKey(installation.installation));
        }
    }
}