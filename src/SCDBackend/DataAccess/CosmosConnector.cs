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
        private static readonly CosmosConnector instance = new CosmosConnector();
        private readonly CosmosConnnectorPreset devPreset = new CosmosConnnectorPreset(Db.Dev);
        private readonly CosmosConnnectorPreset testPreset = new CosmosConnnectorPreset(Db.Test);
        private readonly CosmosConnnectorPreset integrationTestPreset = new CosmosConnnectorPreset(Db.Test_integration);

        public static CosmosConnector Instance { get { return instance; } }
        public CosmosConnnectorPreset CCP { get; private set; }

        static CosmosConnector() { }
        public CosmosConnector()
        {
            CCP = devPreset;
        }

        public void ConfigureTest()
        {
            if (CCP != testPreset)
                CCP = testPreset;
        }

        public void ConfigureIntegrationTest()
        {
            if (CCP != integrationTestPreset)
                CCP = integrationTestPreset;
        }

        public void ConfigureDev()
        {
            if (CCP != devPreset)
                CCP = devPreset;
        }

        public async Task<List<Installation>> GetInstallationsAsync()
        {
            await CCP.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c");

            FeedIterator<Installation> queryResultSetIterator = CCP.Containers["dummyInstallations"].GetItemQueryIterator<Installation>(qd);
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
            await CCP.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c WHERE c.name = @name")
                .WithParameter("@name", name);

            FeedIterator<Installation> queryResultSetIterator = CCP.Containers["dummyInstallations"].GetItemQueryIterator<Installation>(qd);
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

        // TODO check if installation exists
        public async Task CreateInstallationAsync(Installation installation) 
        {
            await CCP.EstablishConnection();
            Container c = CCP.Containers["dummyInstallations"];
            var installationItemResponse = await c.CreateItemAsync<Installation>(installation, new PartitionKey(installation.installation));
        }

        // overload for installation copy
        public async Task CreateInstallationAsync(InstallationCopy installation)
        {
            
            await CCP.EstablishConnection();
            Container c = CCP.Containers["dummyInstallations"];
            var installationItemResponse = await c.CreateItemAsync<InstallationCopy>(installation, new PartitionKey(installation.installation));
        }

        public async Task<List<Subscription>> GetSubscriptions()
        {
            await CCP.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c");
            Container c = CCP.Containers["subscriptions"];

            FeedIterator<Subscription> queryResultSetIterator = c.GetItemQueryIterator<Subscription>(qd);
            List<Subscription> res = new List<Subscription>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Subscription> currentResultSet = await queryResultSetIterator.ReadNextAsync();

                foreach(Subscription s in currentResultSet)
                {
                    res.Add(s);
                }
            }
            return res;
        }

        public async Task<Subscription> GetSubscription(string id)
        {
            await CCP.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c WHERE c.id = @id").WithParameter("@id", id);
            Container c = CCP.Containers["subscriptions"];

            FeedIterator<Subscription> queryResultSetIterator = c.GetItemQueryIterator<Subscription>(qd);
            Subscription sub = null;

            if (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Subscription> currentResultSet = await queryResultSetIterator.ReadNextAsync();

                foreach(Subscription s in currentResultSet)
                {
                    sub = s;
                }
            }
            return sub;
        }

        public async Task<List<Client>> GetClients()
        {
            await CCP.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c");
            Container c = CCP.Containers["clients"];

            FeedIterator<Client> queryResultSetIterator = c.GetItemQueryIterator<Client>(qd);
            List<Client> res = new List<Client>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Client> currentResultSet = await queryResultSetIterator.ReadNextAsync();

                foreach(Client cl in currentResultSet)
                {
                    res.Add(cl);
                }
            }
            return res;
        }

        public async Task<Client> GetClient(string id)
        {
            await CCP.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c WHERE c.id = @id").WithParameter("@id", id);
            Container c = CCP.Containers["clients"];

            FeedIterator<Client> queryResultSetIterator = c.GetItemQueryIterator<Client>(qd);
            Client client = null;

            if (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Client> currentResultSet = await queryResultSetIterator.ReadNextAsync();

                foreach(Client cl in currentResultSet)
                {
                    client = cl;
                }
            }
            return client;
        }

        public async Task<string> GetItemId(string name)
        {
            await CCP.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT VALUE c.id FROM c WHERE c.name = @name")
                .WithParameter("@name", name);

            FeedIterator<string> queryResultSetIterator = CCP.Containers["dummyInstallations"].GetItemQueryIterator<string>(qd);
            string instId = "0";

            if (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<string> res = await queryResultSetIterator.ReadNextAsync();
                foreach (string i in res)
                {
                    instId = i;
                }
            }
            return instId;
        }
        

        public async Task<bool> StartInstallation(string instName)
        {
            await CCP.EstablishConnection();
            Container c = CCP.Containers["dummyInstallations"];

            Installation toReplace = null;
            toReplace = await GetInstallationAsync(instName);

            if (toReplace == null)
                return false;

            toReplace.status = "started";

            string toReplaceId = await GetItemId(instName);

            if (toReplaceId == "0")
                return false;


            await c.ReplaceItemAsync<Installation>(toReplace, toReplaceId, new PartitionKey(toReplace.installation));
            return true;
        }

        public async Task<bool> StopInstallation(string instName)
        {
            await CCP.EstablishConnection();
            Container c = CCP.Containers["dummyInstallations"];

            Installation toReplace = null;
            toReplace = await GetInstallationAsync(instName);

            if (toReplace == null)
                return false;

            toReplace.status = "stopped";

            string toReplaceId = await GetItemId(instName);

            if (toReplaceId == "0")
                return false;


            await c.ReplaceItemAsync<Installation>(toReplace, toReplaceId, new PartitionKey(toReplace.installation));
            return true;
        }

        public async Task DeleteInstallation(Installation inst)
        {
            await CCP.EstablishConnection();
            Container c = CCP.Containers["dummyInstallations"];
            var documentLink = await GetInstallationAsync(inst.name);
            Console.WriteLine(documentLink.id);
            string id = await GetItemId(inst.name);
            var installationItemResponse = await c.DeleteItemAsync<Installation>(id, new PartitionKey(documentLink.installation));
        }

        public async Task CreateSubscriptionAsync(Subscription subscription)
        {
            await CCP.EstablishConnection();
            Container c = CCP.Containers["subscriptions"];
            await c.CreateItemAsync<Subscription>(subscription, new PartitionKey(subscription.subscriptions));
        }

        public async Task CreateClientAsync(Client client)
        {
            await CCP.EstablishConnection();
            Container c = CCP.Containers["clients"];
            await c.CreateItemAsync<Client>(client, new PartitionKey(client.clients));
        }
    }
}