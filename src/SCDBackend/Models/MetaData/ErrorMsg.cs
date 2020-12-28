using System;

namespace SCDBackend.DataAccess.MetaData
{
    [Serializable]
    public class ErrorMessage
    {
        public string msg { get; set; }
        public int httpStatusCode { get; set; }
    }

}