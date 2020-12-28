using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;    
    
    
namespace SCDBackend.Models
{
    [Serializable]
    public class Client
    {
        public string name { get; set; }
        public string id { get; set; }

        public string clients { get; }
        
        public Client(string id, string name)
        {
            this.id = id;
            this.name = name;
            this.clients = "PARTITIONKEY";
        }
    }
}