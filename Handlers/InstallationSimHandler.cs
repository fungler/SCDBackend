using SCDBackend.Models;
using System;
using Xunit.Abstractions;

namespace SCDBackend.Handlers
{
    public class InstallationSimHandler
    {
        private static InstallationSimHandler instance = new InstallationSimHandler();
        static InstallationSimHandler() { }
        private InstallationSimHandler() { }



        public static InstallationSimHandler GetInstance() 
        { 
            return instance; 
        }

        public InstallationSim createSuccessfulInstallation(int startTime, int runTime, ITestOutputHelper output)
        {
            InstallationSim installationSuccess = new InstallationSim(Guid.NewGuid(), startTime, runTime, false, 0, output);
            return installationSuccess;
        }

        public InstallationSim createFailedInstallation(int startTime, int runTime, int failTime, ITestOutputHelper output)
        {
            InstallationSim installationFail = new InstallationSim(Guid.NewGuid(), startTime, runTime, true, failTime, output);
            return installationFail;
        }

    }
}
