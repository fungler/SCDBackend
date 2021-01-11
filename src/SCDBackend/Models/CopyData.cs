using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCDBackend.Models
{
    [Serializable]
    public class CopyData
    {
        public string oldName { get; set; }
        public string newName { get; set; }

        public CopyData(string oldName, string newName)
        {
            this.oldName = oldName;
            this.newName = newName;
        }
    }
}
