using Microsoft.Azure.Cosmos;
using SCDBackend.DataAccess;
using SCDBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCDBackend.IntegrationTests.TestClasses
{
    public class IntegrationSetup
    {
        public CosmosConnector testConnector { get; private set; }
        public List<Installation> Installations { get; private set; }
        public List<Subscription> Subscriptions { get; private set; }
        public List<Client> Clients { get; private set; }

        private bool dataCreated = false;

        public IntegrationSetup()
        {
            testConnector = CosmosConnector.Instance;
            testConnector.ConfigureIntegrationTest();
            testConnector.CCP.EstablishConnection().Wait();
        }

        public async Task<bool> CreateTestData()
        {
            if (!dataCreated)
            {
                try
                {
                    Installations = new List<Installation>();
                    Subscriptions = new List<Subscription>();
                    Clients = new List<Client>();

                    Subscription s1 = new Subscription("d6741d73-abee-41f5-b0f5-886bd849a2b2", "Presales West Europe");
                    Subscription s2 = new Subscription("2", "Presales East US 2");
                    Subscription s3 = new Subscription("3", "Education West Europe");

                    Client c1 = new Client("1", "Danske Bank");
                    Client c2 = new Client("2", "Arbejdernes Landsbank");
                    Client c3 = new Client("3", "Nordea");

                    Subscriptions.Add(s1);
                    Subscriptions.Add(s2);
                    Subscriptions.Add(s3);

                    Clients.Add(c1);
                    Clients.Add(c2);
                    Clients.Add(c3);

                    Installations.Add(new Installation("inst1", "1", s1, c3, "started"));
                    Installations.Add(new Installation("inst2", "2", s2, c2, "starting"));
                    Installations.Add(new Installation("inst3", "3", s3, c2, "stopped"));
                    Installations.Add(new Installation("inst4", "4", s3, c1, "cold"));
                    Installations.Add(new Installation("inst5", "5", s2, c1, "none"));
                    Installations.Add(new Installation("inst6", "6", s1, c3, "cold"));

                    foreach (var i in Installations)
                    {
                        await testConnector.CreateInstallationAsync(i);
                    }

                    foreach (var c in Clients)
                    {
                        await testConnector.CreateClientAsync(c);
                    }

                    foreach (var s in Subscriptions)
                    {
                        await testConnector.CreateSubscriptionAsync(s);
                    }

                    dataCreated = true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> CleanUpTestData()
        {
            try
            {
                var tasks = new List<Task>();
                Dictionary<string, Container> containers = testConnector.CCP.Containers;
                foreach (var c in containers)
                {
                    tasks.Add(Task.Run(() => c.Value.DeleteContainerAsync()));
                }

                Task t = Task.WhenAll(tasks);

                t.Wait();

                return t.IsCompletedSuccessfully;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
