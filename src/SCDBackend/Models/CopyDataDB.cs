using System;

namespace SCDBackend.Models
{
    [Serializable]
    public class CopyDataDB
    {
        public string oldName { get; set; }
        public string newName { get; set; }
        public string copyMethod { get; set; }
        public Client client { get; set; }

        public Subscription Subscription { get; set; }
    }
}
