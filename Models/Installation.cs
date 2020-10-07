using System;
using System.Collections.Generic;

namespace SCDBackend.Models
{
    [Serializable]
    public class Installation
    {
        public Installation(string name, string fullAddress, string subscription, List<Client> clients) 
        {
            this.id = Guid.NewGuid();
            this.name = name;
            this.installation = "PARTITIONKEY"; // Manually set since we are not partitioning the database
            this.fullAddress = fullAddress;
            this.subscription = subscription;
            this.clients = clients;
        }
        
        public Guid id { get; }
        public string installation { get; }
        public string name { get; set; }

        public string fullAddress { get; set; }

        public string subscription { get; set; }
        public List<Client> clients { get; set; }
    }

    [Serializable]
    public class Client
    {
        public Client(string name)  
        {
            this.name = name;
            this.id = Guid.NewGuid();
        }

        public string name { get; set; }
        public Guid id { get; }
    }
}