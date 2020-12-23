using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCDBackend.Models
{
    [Serializable]
    public class SDDResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public string installation_status { get; set; }
    }
}
