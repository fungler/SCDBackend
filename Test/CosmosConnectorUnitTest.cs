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

        private static readonly string PrimaryKey = "PcehWtM3wPQmnqPrBXE0KYYQrmVRLpw3MluwlMWXI93lS0XUbmnC8oHC2UJRU1ggBl5WRm68a4GH8NGSS5EZQw==";

        private static readonly string Endpoint = "https://test-database1.documents.azure.com:443/";

        private static string databaseId = "frontend_test";
        private static string containerId = "dummyInstallations";
        private static Microsoft.Azure.Cosmos.Database database;
        private static Container container;
        private static CosmosClient cosmosClient;

        public CosmosConnector Db { get; private set; }

        public List<Installation> Installations { get; private set; }

        private bool dataCreated = false;

        public DatabaseFixture() 
        {
            Dictionary<string, string> a = new Dictionary<string, string>();
            a.Add("installation", "/installation");
            CosmosConnnectorCreator c = new CosmosConnnectorCreator(Endpoint, PrimaryKey, databaseId, a);
            
            Db = new CosmosConnector(c);
        }

        public async Task CreateTestData()
        {
            if(!dataCreated)
            {
            // await Db.cosmosClient.GetDatabase(databaseId).CreateContainerAsync(new ContainerProperties(containerId, "/installation"));

                Installations = new List<Installation>();
                Installations.Add(new Installation("inst1", "1", null, null, "running"));
                Installations.Add(new Installation("inst2", "2", null, null, "starting"));
                Installations.Add(new Installation("inst3", "3", null, null, "stopped"));
                Installations.Add(new Installation("inst4", "4", null, null, "cold"));
                Installations.Add(new Installation("inst5", "5", null, null, "none"));
                Installations.Add(new Installation("inst6", "6", null, null, "cold"));

                foreach(var i in Installations)
                {
                    await Db.CreateInstallationAsync(i);
                }

                dataCreated = true;
            }
        }

        // Delete container after all tests to ensure that the new dataset is clean
        public void Dispose()
        {   
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
        public void TestFixture()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task TestGetInstallationsAsync()
        {
            await fixture.CreateTestData();
            
            // Installation objects created in DatabaseFixture
            List<Installation> Inst = fixture.Installations;

            // Fetched from db
            List<Installation> installations = await Task.Run<List<Installation>>(async () =>
            {
                return await fixture.Db.GetInstallationsAsync();
            });
            output.WriteLine(installations.Count.ToString());
            Assert.NotNull(installations);
            Assert.True(installations.Count >= 6);
        }

        [Fact]
        public async Task TestGetInstallationAsync()
        {
            await fixture.CreateTestData();

            var installation = await Task.Run<Installation>(async () => 
            {
                return await fixture.Db.GetInstallationAsync("inst1");
            });

            Assert.NotNull(installation);
            Assert.True(installation.fullAddress.Equals("1"));
            Assert.True(installation.state.Equals("running"));
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
        public async Task TestGetSubscriptions()
        {

        }

        [Fact]
        public async Task TestGetSubscription()
        {
            
        }

        [Fact]
        public async Task TestGetClients()
        {

        }

        [Fact]
        public async Task TestGetClient()
        {

        }

        [Fact]
        public async Task TestStartInstallation()
        {

        }

        [Fact]
        public async Task TestStopInstallation()
        {

        }

        [Fact]
        public async Task TestDeleteInstallation()
        {

        }

    }
}
