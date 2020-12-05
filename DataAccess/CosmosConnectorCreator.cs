using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using System;

namespace SCDBackend.DataAccess
{
    public class CosmosConnnectorCreator
    {   

        private string Endpoint { get; }
        private string PrimaryKey { get; }

        private string DatabaseId { get; }

        // containerId, partitionkey
        private Dictionary<string, string> ContainerData { get; }

        public Dictionary<string, Container> Containers { get; set; }

        public CosmosClient CosmosClient;

        public Database Database;

        public CosmosConnnectorCreator(string Endpoint, string PrimaryKey, string DatabaseId, Dictionary<string, string> ContainerData)
        {
            this.Endpoint = Endpoint;
            this.PrimaryKey = PrimaryKey;
            this.DatabaseId = DatabaseId;
            this.ContainerData = ContainerData;
            this.Containers = new Dictionary<string, Container>();
        }

        private async Task InitAsync()
        {
            if (CosmosClient == null || Database == null || Containers == null)
            {
                CosmosClient = new CosmosClient(Endpoint, PrimaryKey);
                Database = await CosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
                
                foreach(KeyValuePair<string, string> entry in ContainerData)
                {
                    Container c = await Database.CreateContainerIfNotExistsAsync(new ContainerProperties(entry.Key, entry.Value));
                    Containers.Add(entry.Key, c);
                }
            }
        }

        public async Task EstablishConnection()
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
    }
}