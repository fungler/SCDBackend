using SCDBackend.Models;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace UnitTest
{
    public class UnitTest1
    {

        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task Test1()
        {
            InstallationSim installationSuccess = new InstallationSim(new Guid(), 5000, 10000, false, 0, output);
            InstallationSim installationSuccess2 = new InstallationSim(new Guid(), 2000, 6000, false, 0, output);

            // use whenall to actually make it run async and speed up mulitple setups
            await Task.WhenAll(Task.Run(() => installationSuccess.runSetup()), Task.Run(() => installationSuccess2.runSetup()));

            Assert.Equal(StatusType.STATUS_FINISHED_SUCCESS, installationSuccess.status);
            Assert.Equal(StatusType.STATUS_FINISHED_SUCCESS, installationSuccess2.status);
        }

        [Fact]
        public async Task Test2()
        {
            InstallationSim installationFail = new InstallationSim(new Guid(), 5000, 10000, true, 2000, output);

            await Task.Run(async () =>
            {
                await installationFail.runSetup();
            });

            Assert.Equal(StatusType.STATUS_FINISHED_FAILED, installationFail.status);
        }
    }
}
