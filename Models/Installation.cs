using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SCDBackend.Models
{
    [Serializable]
    public class Installation
    {

        public Installation(string name, string fullAddress, Subscription subscription, Client client)
        {
            this.id = Guid.NewGuid();
            this.name = name;
            this.installation = "PARTITIONKEY"; // Manually set since we are not partitioning the database
            this.fullAddress = fullAddress;
            this.subscription = subscription;
            this.copyMethod = "cold";
            this.client = client;
        }

        public Guid id { get; }
        public string installation { get; }
        public string name { get; set; }

        public string fullAddress { get; set; }

        public Subscription subscription { get; set; }
        public string copyMethod { get; set; }
        public Client client { get; set; }
    }

    [Serializable]
    public class InstallationCopy
    {
        public InstallationCopy(string name, string fullAddress, Subscription subscription, string copyMethod, Client client)
        {
            this.id = Guid.NewGuid();
            this.name = name;
            this.installation = "PARTITIONKEY"; // Manually set since we are not partitioning the database
            this.fullAddress = fullAddress;
            this.subscription = subscription;
            this.copyMethod = copyMethod;
            this.client = client;
        }

        public Guid id { get; }
        public string installation { get; }
        public string name { get; set; }

        public string fullAddress { get; set; }

        public Subscription subscription { get; set; }
        public string copyMethod { get; set; }
        public Client client { get; set; }
    }


    [Serializable]
    public class Client
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Network
    {
        public string vnetResourceGroupName { get; set; }
        public string vnetName { get; set; }
        public string subnetName { get; set; }
    }

    public class Secrets
    {
        public string keyVaultName { get; set; }
        public string adAutomationAccountPrefix { get; set; }
        public string dbAutomationAccountPrefix { get; set; }
    }

    public class DatabaseServer
    {
        public string hostFqdn { get; set; }
        public string ipAddress { get; set; }
        public string protocol { get; set; }
        public int port { get; set; }
        public string serviceName { get; set; }
        public int ordsHttpsPort { get; set; }
    }

    public class Database
    {
        public string pdbName { get; set; }
        public string creationMethod { get; set; }
    }

    public class OrderManager
    {
        public string sqlServerFqdn { get; set; }
        public string sqlDatabaseName { get; set; }
    }

    public class StorageAccount
    {
        public string subscriptionId { get; set; }
        public string resourceGroupName { get; set; }
        public string name { get; set; }
        public string containerName { get; set; }
        public string path { get; set; }
    }

    public class FileSystem
    {
        public StorageAccount storageAccount { get; set; }
    }

    public class Database2
    {
        public string type { get; set; }
        public string dbServerName { get; set; }
        public string dbInstanceName { get; set; }
        public string pdbName { get; set; }
    }

    public class Source
    {
        public FileSystem fileSystem { get; set; }
        public Database2 database { get; set; }
    }

    public class Tags
    {
        public string creator { get; set; }
        [JsonPropertyName("cost-center")]
        public string costcenter { get; set; }
        [JsonPropertyName("legal-unit")]
        public string legalunit { get; set; }
        public string environment { get; set; }
        [JsonPropertyName("vm-backup")]
        public string vmbackup { get; set; }
        [JsonPropertyName("sql-backup")]
        public string sqlbackup { get; set; }
        [JsonPropertyName("customer-data")]
        public string customerdata { get; set; }
        [JsonPropertyName("personal-data")]
        public string personaldata { get; set; }
        [JsonPropertyName("expiration-date")]
        public string expirationdate { get; set; }
        [JsonPropertyName("service-window")]
        public string servicewindow { get; set; }
        public string application { get; set; }
        [JsonPropertyName("client-shortname")]
        public string clientshortname { get; set; } 
    }

    public class InternationalSettings
    {
        public string inputLanguageId { get; set; }
        public string format { get; set; }
        public string systemLocale { get; set; }
        public int geoId { get; set; }
    }

    public class Role
    {
        public string name { get; set; }
        public string additional { get; set; }
    }

    public class Instance
    {
        public string vmSize { get; set; }
        public string vmImageSku { get; set; }
        public string licenseType { get; set; }
        public string configurationMode { get; set; }
        public List<Role> roles { get; set; }
    }

    public class VmScaleSet
    {
        public string name { get; set; }
        public string computerNamePrefix { get; set; }
        public int instanceCount { get; set; }
        public Instance instance { get; set; }
    }

    public class Role2
    {
        public string name { get; set; }
        public string satag { get; set; }
        public string additional { get; set; }
    }

    public class Vm
    {
        public string name { get; set; }
        public string vmSize { get; set; }
        public string vmImageSku { get; set; }
        public string licenseType { get; set; }
        public string configurationMode { get; set; }
        public List<Role2> roles { get; set; }
    }

    public class Inst
    {
        public string name { get; set; }
        public string resourceGroupName { get; set; }
        public string location { get; set; }
        public string storageAccountName { get; set; }
        public string adminsGroupName { get; set; }
        public string fullyQualifiedGmsaName { get; set; }
        public string gmsaHostsGroupDistinguishedName { get; set; }
        public string gmsaHostsGroupName { get; set; }
        public string gmsaName { get; set; }
        public string organizationalUnitDistinguishedName { get; set; }
        public string serverLocalAdminGroup { get; set; }
        public string usersGroupName { get; set; }
        public string keyVaultName { get; set; }
        public DatabaseServer databaseServer { get; set; }
        public Database database { get; set; }
        public string localFileSharePath { get; set; }
        public string netRootUncPath { get; set; }
        public OrderManager orderManager { get; set; }
        public string mucsPort { get; set; }
        public string svcDirectoryServicePort { get; set; }
        public Source source { get; set; }
        public string state { get; set; }
        public string iconColor { get; set; }
        public Tags tags { get; set; }
        public InternationalSettings internationalSettings { get; set; }
        public List<VmScaleSet> vmScaleSets { get; set; }
        public List<Vm> vms { get; set; }
    }

    public class InstallationRoot
    {
        public string azureTenant { get; set; }
        public string subscriptionId { get; set; }
        public string domainName { get; set; }
        public Network network { get; set; }
        public Secrets secrets { get; set; }
        public Inst installation { get; set; }
    }
}