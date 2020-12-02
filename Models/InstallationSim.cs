using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;

// Class for simulation of installations
namespace SCDBackend.Models
{
    public class InstallationSim
    {

        private readonly ITestOutputHelper output;

        public Guid id { get; set; }
        public StatusType status { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        // config
        private int startTimeMs { get; set; } // how long should the setup/init run for
        private int runTimeMs { get; set; } // how long should the setup run for
        private int stopTimeMs { get; set; } // how long should the setup stop for
        private bool shouldFail { get; set; } // should this installation fail
        private int failTimeMs { get; set; } // how long till the setup should fail


        public InstallationSim(Guid id, int startTime,  int runTime, bool shouldFail, int failTime)
        {
            this.id = id;
            this.status = StatusType.STATUS_COLD;
            this.creationDate = DateTime.Now;
            this.runTimeMs = runTime;
            this.startTimeMs = startTime;
            this.shouldFail = shouldFail;
            this.failTimeMs = failTime;
        }

        public InstallationSim(Guid id, int startTime, int runTime, bool shouldFail, int failTime, ITestOutputHelper output)
        {
            this.id = id;
            this.status = StatusType.STATUS_COLD;
            this.creationDate = DateTime.Now;
            this.runTimeMs = runTime;
            this.startTimeMs = startTime;
            this.shouldFail = shouldFail;
            this.failTimeMs = failTime;
            this.output = output;
        }

        public async Task<InstallationSim> runSetup()
        {
            output.WriteLine(id + " - starting..");

            startDate = DateTime.Now;
            output.WriteLine(id + " - START: " + startDate);

            status = StatusType.STATUS_STARTING;
            output.WriteLine(id + " - STATUS: " + status);
            await Task.Delay(startTimeMs);


            status = StatusType.STATUS_RUNNING;
            output.WriteLine(id + " - running..");
            output.WriteLine(id + " - STATUS: " + status);

            if (shouldFail)
            {
                output.WriteLine(id + " Failing..");
                await Task.Delay(failTimeMs);
                endDate = DateTime.Now;
                output.WriteLine(id + " - END: " + endDate);
                status = StatusType.STATUS_FINISHED_FAILED;
                output.WriteLine(id + " - STATUS: " + status);
                return this;
            }
            else
            {
                output.WriteLine(id + " Working..");
                await Task.Delay(runTimeMs);
                endDate = DateTime.Now;
                output.WriteLine(id + " - END: " + endDate);
                status = StatusType.STATUS_FINISHED_SUCCESS;
                output.WriteLine(id + " - STATUS: " + status);
                return this;
            }

        }

    }
}
