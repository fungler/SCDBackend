using System;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using SCDBackend.DataAccess;
using System.Collections.Generic;
using SCDBackend.Models;

namespace CosmosConnectorUnitTest
{
    public class DatabaseFixture : IDisposable
    {
        public CosmosConnector Db { get; private set; }

        public List<Installation> Installations { get; private set; }
        public List<Subscription> Subscriptions { get; private set; }
        public List<Client> Clients { get; private set; }

        private bool dataCreated = false;

        public DatabaseFixture() 
        {
            Db = CosmosConnector.Instance;
            Db.ConfigureTest();

            try 
            {
                Task.Run(()=> CreateTestData()).Wait();
            } catch {}

        }

        public async Task CreateTestData()
        {
            // Data is only created once
            if(!dataCreated)
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

                foreach(var i in Installations)
                {
                    await Db.CreateInstallationAsync(i);
                }

                foreach(var c in Clients)
                {
                    await Db.CreateClientAsync(c);
                }

                foreach(var s in Subscriptions)
                {
                    await Db.CreateSubscriptionAsync(s);
                }

                dataCreated = true;
            }
        }

        // Delete container after all tests to ensure that the new dataset is clean
        public void Dispose()
        {
            var tasks = new List<Task>();
            Dictionary<string, Container> containers = Db.CCP.Containers;
            foreach(var c in containers)
            {
                tasks.Add(Task.Run(() => c.Value.DeleteContainerAsync()));
            }

            Task t = Task.WhenAll(tasks);

            try 
            {
                t.Wait();
            }
            catch {}
        }
    }

    public class CosmosConnectorUnitTest : IClassFixture<DatabaseFixture>
    {
        DatabaseFixture fixture;

        ITestOutputHelper output;

        public CosmosConnectorUnitTest(DatabaseFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
        }

        [Fact]
        public async Task TestGetInstallationsAsync()
        {
            //await fixture.CreateTestData();
            
            // Installation objects created in DatabaseFixture
            List<Installation> Inst = fixture.Installations;

            // Fetched from db
            List<Installation> installations = await Task.Run<List<Installation>>(async () =>
            {
                return await fixture.Db.GetInstallationsAsync();
            });
            output.WriteLine(installations.Count.ToString());
            Assert.NotNull(installations);
            //Assert.True(installations.Count >= 6);
            foreach(var inst in installations)
            {
                Assert.NotNull(inst.client);
                Assert.NotNull(inst.subscription);
            }
        }

        // TODO: Doesn't work, FIX!
        public async Task TestGetInstallationAsync()
        {
            //await fixture.CreateTestData();

            var installation = await Task.Run<Installation>(async () => 
            {
                return await fixture.Db.GetInstallationAsync("inst1");
            });

            Assert.NotNull(installation);
            Assert.True(installation.fullAddress.Equals("1"));
            Assert.True(installation.state.Equals("started"));
        }

        public async Task TestCreateInstallationAsync()
        {
            Installation inst = new Installation("Inst7", "7", null, null, "Cold");
            await fixture.Db.CreateInstallationAsync(inst);

            Installation fetched = await fixture.Db.GetInstallationAsync(inst.name);

            Assert.True(inst.name.Equals(fetched.name));
            Assert.True(inst.fullAddress.Equals(fetched.fullAddress));
        }

        [Fact]
        public async Task TestGetSubscriptionsAsync()
        {
            //await fixture.CreateTestData();
            List<Subscription> subs = fixture.Subscriptions;

            List<Subscription> dbSubs = await fixture.Db.GetSubscriptions();

            Assert.True(dbSubs.Count == 3);

            Subscription sub = subs[1];
            bool found = false;

            foreach(var s in dbSubs)
            {
                if(s.name == sub.name && s.id == sub.id)
                {
                    found = true;
                }
            }
            Assert.True(found);
        }

        [Fact]
        public async Task TestGetSubscriptionAsync()
        {
            //await fixture.CreateTestData();
            Subscription s = fixture.Subscriptions[1];

            Subscription dbSub = await fixture.Db.GetSubscription(s.id);
            Assert.NotNull(dbSub);
            Assert.True(dbSub.name.Equals(s.name));
        }

        [Fact]
        public async Task NegativeTestGetSubscriptionAsync()
        {
            //await fixture.CreateTestData();
            Subscription s = await fixture.Db.GetSubscription("4");
            Assert.Null(s);
        }

        [Fact]
        public async Task TestGetClients()
        {
            //await fixture.CreateTestData();
            List<Client> clients = fixture.Clients;
            List<Client> dbClients = await fixture.Db.GetClients();

            Assert.True(dbClients.Count == 3);

            Client cl = clients[2];

            bool found = false;

            foreach(var c in dbClients)
            {
                if(c.name.Equals(cl.name) && c.id.Equals(cl.id))
                {
                    found = true;
                }
            }

            Assert.True(found);
        }

        [Fact]
        public async Task TestGetClient()
        {
            //await fixture.CreateTestData();
            Client c = fixture.Clients[0];
            Client dbc = await fixture.Db.GetClient(c.id);
            Assert.NotNull(c);
            Assert.True(c.name.Equals(dbc.name));
        }

        [Fact]
        public async Task TestStartInstallation()
        {
            //await fixture.CreateTestData();
            Installation inst = fixture.Installations[3];
            // Make sure installation is not starting/running
            Assert.False(inst.status.Equals("started") || inst.status.Equals("starting")); 
            await fixture.Db.StartInstallation(inst.name);
           
            Installation refetched = await fixture.Db.GetInstallationAsync(inst.name);
            Assert.True(refetched.status.Equals("started"));
        }

        [Fact]
        public async Task TestStopInstallation()
        {
            //await fixture.CreateTestData();
            Installation inst = fixture.Installations[1];
            Assert.False(inst.status.Equals("stopped") || inst.status.Equals("stopping"));
            await fixture.Db.StopInstallation(inst.name);

            Installation refetched = await fixture.Db.GetInstallationAsync(inst.name);
            Assert.True(refetched.status.Equals("stopped"));
        }

        //[Fact]
        public async Task TestDeleteInstallation()
        {
            //await fixture.CreateTestData();
            List<Installation> installations = fixture.Installations;
            var length = installations.Count;
            Installation inst = installations[0];

            await fixture.Db.DeleteInstallation(inst);
            List<Installation> dbInsts = await fixture.Db.GetInstallationsAsync();
            Assert.True(length > dbInsts.Count);
            
            bool found = false;
            foreach(var i in dbInsts)
            {
                if(i.name.Equals(inst.name) && i.id.Equals(inst.id))
                {
                    found = true;
                }
            }

            Assert.False(found);

        }

    }
}
