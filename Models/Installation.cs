using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCDBackend.Models
{
    [Serializable]
    public class Installation
    {
        public Installation(string id, string name, string status, string active_users)  
        {
            this.id = id;
            this.name = name;
            this.status = status;
            this.active_users = active_users;
        }
        public string id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string active_users { get; set; }
    }
}
