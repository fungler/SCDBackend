using System;

namespace SCDBackend.Models
{
    [Serializable]
    public class Installation
    {
        // Used for Json serialization
        public Installation(){}

        public Installation(string name, string fullAddress, Subscription subscription, Client client, string state)
        {
            this.id = Guid.NewGuid();
            this.name = name;
            this.installation = "PARTITIONKEY"; // Manually set since we are not partitioning the database
            this.fullAddress = fullAddress;
            this.subscription = subscription;
            this.copyMethod = "cold";
            this.client = client;
            this.status = "cold";
            this.state = state;
        }

        public Installation(string name, string fullAddress, Subscription subscription, string copyMethod, Client client)
        {
            this.id = Guid.NewGuid();
            this.name = name;
            this.installation = "PARTITIONKEY"; // Manually set since we are not partitioning the database
            this.fullAddress = fullAddress;
            this.subscription = subscription;
            this.copyMethod = copyMethod;
            this.client = client;
            this.status = "Cold";
            this.state = "none";
        }

        public Guid id { get; set; }
        public string installation { get; set; }
        public string name { get; set; }

        public string fullAddress { get; set; }

        public Subscription subscription { get; set; }
        public string copyMethod { get; set; }
        public Client client { get; set; }
        public string status { get; set; }
        public string state { get; set; }
    }
}