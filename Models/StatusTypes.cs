using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCDBackend.Models
{
    public enum StatusType
    {
        STATUS_NONE,
        STATUS_COLD,
        STATUS_STARTING,
        STATUS_RUNNING,
        STATUS_STOPPING,
        STATUS_STOPPED,
        STATUS_FINISHED_FAILED,
        STATUS_FINISHED_SUCCESS
    }
}
