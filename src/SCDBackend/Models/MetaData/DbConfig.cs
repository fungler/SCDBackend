using System;

namespace SCDBackend.Models.MetaData
{
    [Serializable]
    public class DbConfig
    {
        public string endpoint {get; set;}
        public string key {get; set;}
        public string databaseId { get; set; }
    }
}