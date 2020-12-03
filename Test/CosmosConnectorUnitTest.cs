using System;
using Xunit;
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

        public DatabaseFixture() 
        {
            Db = new CosmosConnector(Endpoint, PrimaryKey, databaseId, containerId);
        }

        public async Task CreateTestData()
        {
            Installation inst1 = new Installation("inst1", "1234", null, null, "none");
            Installation inst2 = new Installation("inst2", "1234", null, null, "none");
            Installation inst3 = new Installation("inst3", "1234", null, null, "none");
            Installation inst4 = new Installation("inst4", "1234", null, null, "none");
            Installation inst5 = new Installation("inst5", "1234", null, null, "none");
            Installation inst6 = new Installation("inst6", "1234", null, null, "none");

            await Db.CreateInstallationAsync(inst1);
            await Db.CreateInstallationAsync(inst2);
            await Db.CreateInstallationAsync(inst3);
            await Db.CreateInstallationAsync(inst4);
            await Db.CreateInstallationAsync(inst5);
            await Db.CreateInstallationAsync(inst6);
        }

        public void Dispose()
        {
            Db.cosmosClient.GetContainer(databaseId, containerId).DeleteContainerAsync();
        }
    }
    public class CosmosConnectorUnitTest : IClassFixture<DatabaseFixture>
    {
        DatabaseFixture fixture;

        public CosmosConnectorUnitTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
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

            List<Installation> installations = await Task.Run<List<Installation>>(async () =>
            {
                return await fixture.Db.GetInstallationsAsync();
            });

            Assert.NotNull(installations);
            Assert.True(installations.Count == 6);
        }
    }
}
