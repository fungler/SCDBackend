using SCDBackend.Handlers;
using SCDBackend.Models;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace UnitTest
{
    public class UnitTest
    {
        private readonly InstallationSimHandler simHandler = InstallationSimHandler.GetInstance();
        private readonly ITestOutputHelper output;

        public UnitTest(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public async Task createInstallation_installations_success()
        {
            // sending output as func param to avoid test output bugs compared to making a class field in simHandler
            InstallationSim i1 = simHandler.createSuccessfulInstallation(1000, 1000, output);
            InstallationSim i2 = simHandler.createSuccessfulInstallation(2000, 2000, output);

            // use whenall to make it run parallel and speed up mulitple setups
            await Task.WhenAll(
                Task.Run(() => i1.runSetup()),
                Task.Run(() => i2.runSetup())
            );

            Assert.Equal(StatusType.STATUS_FINISHED_SUCCESS, i1.status);
            Assert.Equal(StatusType.STATUS_FINISHED_SUCCESS, i2.status);
        }


        [Fact]
        public async Task createInstallation_installation_fail()
        {
            InstallationSim i1 = simHandler.createFailedInstallation(1000, 1000, 1000, output);

            await Task.Run(async () =>
            {
                await i1.runSetup();
            });

            Assert.Equal(StatusType.STATUS_FINISHED_FAILED, i1.status);
        }

        [Fact]
        public async Task createInstallation_installations_success_and_fail()
        {
            InstallationSim i1 = simHandler.createSuccessfulInstallation(1000, 1000, output);
            InstallationSim i2 = simHandler.createFailedInstallation(1000, 1000, 1000, output);

            // use whenall to make it run parallel and speed up mulitple setups
            await Task.WhenAll(
                Task.Run(() => i1.runSetup()),
                Task.Run(() => i2.runSetup())
            );

            Assert.Equal(StatusType.STATUS_FINISHED_SUCCESS, i1.status);
            Assert.Equal(StatusType.STATUS_FINISHED_FAILED, i2.status);
        }
    }
}
