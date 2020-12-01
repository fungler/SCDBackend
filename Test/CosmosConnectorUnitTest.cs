using System;
using Xunit;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using SCDBackend.DataAccess;

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

        public void Dispose()
        {
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
        public void TestGetInstallationsAsync()
        {

            Assert.True(true);
        }
    }
}
