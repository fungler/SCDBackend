using System;

namespace SCDBackend.Models.MetaData
{
    [Serializable]
    public class JsonMessage
    {
        public string msg { get; set; }

        public JsonMessage(string msg)
        {
            this.msg = msg;
        }
    }
}