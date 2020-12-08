using System;
namespace SCDBackend.Models 
{
    [Serializable]
    public class Subscription
    {
        	public string id { get; set; }
            public string name { get; set; }

            public string subscription { get; }

            public Subscription (string id, string name)
            {
                this.id = id;
                this.name = name;
                this.subscription = "PARTITIONKEY";
            }
    }
}