using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCDBackend.Models
{
    [Serializable]
    public class CopyDataDB
    {
        public string oldName { get; set; }
        public string newName { get; set; }
        public string copyMethod { get; set; }
        public List<Client> clients { get; set; }
    }
}
