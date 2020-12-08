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

        public CosmosConnnectorCreator CCC { get; }

        public CosmosConnector(CosmosConnnectorCreator c)
        {
            CCC = c;

        }

        public async Task<List<Installation>> GetInstallationsAsync()
        {
            await CCC.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c");

            FeedIterator<Installation> queryResultSetIterator = CCC.Containers["dummyInstallations"].GetItemQueryIterator<Installation>(qd);
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
            await CCC.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c WHERE c.name = @name")
                .WithParameter("@name", name);

            FeedIterator<Installation> queryResultSetIterator = CCC.Containers["dummyInstallations"].GetItemQueryIterator<Installation>(qd);
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
            await CCC.EstablishConnection();
            Container c = CCC.Containers["dummyInstallations"];
            var installationItemResponse = await c.CreateItemAsync<Installation>(installation, new PartitionKey(installation.installation));
        }

        // overload for installation copy
        public async Task CreateInstallationAsync(InstallationCopy installation)
        {
            
            await CCC.EstablishConnection();
            Container c = CCC.Containers["dummyInstallations"];
            var installationItemResponse = await c.CreateItemAsync<InstallationCopy>(installation, new PartitionKey(installation.installation));
        }

        public async Task<List<Subscription>> GetSubscriptions()
        {
            await CCC.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c");
            Container c = CCC.Containers["subscriptions"];

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
            await CCC.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c WHERE c.id = @id").WithParameter("@id", id);
            Container c = CCC.Containers["subscriptions"];

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
            await CCC.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c");
            Container c = CCC.Containers["clients"];

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
            await CCC.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT * FROM c WHERE c.id = @id").WithParameter("@id", id);
            Container c = CCC.Containers["clients"];

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
            await CCC.EstablishConnection();
            QueryDefinition qd = new QueryDefinition("SELECT VALUE c.id FROM c WHERE c.name = @name")
                .WithParameter("@name", name);

            FeedIterator<string> queryResultSetIterator = CCC.Containers["dummyInstallations"].GetItemQueryIterator<string>(qd);
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
        

        public async Task<int> StartInstallation(string instName)
        {
            await CCC.EstablishConnection();
            Container c = CCC.Containers["dummyInstallations"];

            Installation toReplace = null;
            toReplace = await GetInstallationAsync(instName);

            if (toReplace == null)
                return 0;

            toReplace.status = "started";

            string toReplaceId = await GetItemId(instName);

            if (toReplaceId == "0")
                return 0;


            await c.ReplaceItemAsync<Installation>(toReplace, toReplaceId, new PartitionKey(toReplace.installation));
            return 1;
        }

        public async Task<int> StopInstallation(string instName)
        {
            await CCC.EstablishConnection();
            Container c = CCC.Containers["dummyInstallations"];

            Installation toReplace = null;
            toReplace = await GetInstallationAsync(instName);

            if (toReplace == null)
                return 0;

            toReplace.status = "stopped";

            string toReplaceId = await GetItemId(instName);

            if (toReplaceId == "0")
                return 0;


            await c.ReplaceItemAsync<Installation>(toReplace, toReplaceId, new PartitionKey(toReplace.installation));
            return 1;
        }

        public async Task DeleteInstallation(Installation inst)
        {
            await CCC.EstablishConnection();
            Container c = CCC.Containers["dummyInstallations"];
            var documentLink = await GetInstallationAsync(inst.name);
            Console.WriteLine(documentLink.id);
            string id = await GetItemId(inst.name);
            var installationItemResponse = await c.DeleteItemAsync<Installation>(id, new PartitionKey(documentLink.installation));
        }

        public async Task CreateSubscriptionAsync(Subscription subscription)
        {
            await CCC.EstablishConnection();
            Container c = CCC.Containers["subscriptions"];
            await c.CreateItemAsync<Subscription>(subscription, new PartitionKey(subscription.subscription));
        }

        public async Task CreateClientAsync(Client client)
        {
            await CCC.EstablishConnection();
            Container c = CCC.Containers["clients"];
            await c.CreateItemAsync<Client>(client, new PartitionKey(client.client));
        }
    }
}