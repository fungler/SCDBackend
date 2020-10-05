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
        private static Database database;
        private static Container container;

        public CosmosConnector()
        {
        }


        private static async Task InitAsync()
        {
            if (cosmosClient == null || database == null || container == null)
            {
                cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
                database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
                container = await database.CreateContainerIfNotExistsAsync(new ContainerProperties(containerId, "/installation"));
            }
        }

        public async Task<List<Installation>> GetInstallationsAsync()
        {
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

        public async Task establishConnection()
        {
            try
            {
                await InitAsync();
            }
            catch (CosmosException e)
            {
                Console.WriteLine("Cosmos except");
            }
            catch (Exception e)
            {
                Console.WriteLine("Except");
            }
        }

        public async Task<Installation> GetInstallationAsync(string id)
        {
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", id);
            FeedIterator<Installation> queryResultSetIterator = container.GetItemQueryIterator<Installation>(qd);
            Installation inst = null;

            if (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Installation> res = await queryResultSetIterator.ReadNextAsync();
                foreach (Installation i in res)
                {
                    inst = i;
                }
            }
            return inst;
        }

        public async void CreateInstallationAsync(Installation installation) 
        {
            var cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            var container = cosmosClient.GetContainer(databaseId, containerId);
            await container.CreateItemAsync(installation);
            //https://dotnetcoretutorials.com/2020/05/02/using-azure-cosmosdb-with-net-core/
            
            /*Console.Write(container);
            
            try 
            {
                // Read item from container to see if the item exists - do not want to create an already existing document.
                var installationItemResponse = await cosmosClient
                                        .GetContainer(databaseId, containerId)
                                        .ReadItemAsync<Installation>(installation.id, new PartitionKey(installation.id));
                Console.WriteLine("Item in database with id: {0} already exists\n", installationItemResponse.Resource.id);
            } catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // create a new document
                var installationItemResponse = await container.CreateItemAsync<Installation>(installation, new PartitionKey(installation.name));
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", installationItemResponse.Resource.id, installationItemResponse.RequestCharge);
            }
            */
        }
    }
}