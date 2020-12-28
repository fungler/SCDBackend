using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using System;
using System.Configuration;
using System.Buffers;
using Newtonsoft.Json;
using System.IO;
using SCDBackend.Models.MetaData;

namespace SCDBackend.DataAccess
{
    public class CosmosConnnectorPreset
    { 
        private string Endpoint { get; set; }
        private string PrimaryKey { get; set; }

        private string DatabaseId { get; set; }

        // containerId, partitionkey
        private Dictionary<string, string> ContainerData { get; set; }

        public Dictionary<string, Container> Containers { get; set; }

        public CosmosClient CosmosClient;

        public Microsoft.Azure.Cosmos.Database Database;

        public CosmosConnnectorPreset(Db dbType)
        {
            string json;
            using (var sr = new StreamReader("Resources/dbsettings.json"))
            {
                json = sr.ReadToEnd();
            }

            Dictionary<string, DbConfig> c = JsonConvert.DeserializeObject<Dictionary<string, DbConfig>>(json);
            Containers = new Dictionary<string, Container>();
            ContainerData = new Dictionary<string, string>();
            ContainerData.Add("dummyInstallations", "/installation");
            ContainerData.Add("subscriptions", "/subscriptions");
            ContainerData.Add("clients", "/clients");

            if(dbType.Equals(Db.Dev))
            {
                var db = c["dev"];
                this.Endpoint = db.endpoint;
                this.DatabaseId = db.databaseId;
                this.PrimaryKey = db.key;
            }
            else if(dbType.Equals(Db.Test))
            {
                var db = c["test"];
                this.Endpoint = db.endpoint;
                this.DatabaseId = db.databaseId;
                this.PrimaryKey = db.key;
            }
            else if (dbType.Equals(Db.Test_integration))
            {
                var db = c["test_integration"];
                this.Endpoint = db.endpoint;
                this.DatabaseId = db.databaseId;
                this.PrimaryKey = db.key;
            }
        }

        private async Task InitAsync()
        {
            if (CosmosClient == null || Database == null || Containers == null || Containers.Count == 0)
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


    public enum Db
    {
        Dev,
        Test,
        Test_integration
    }
}