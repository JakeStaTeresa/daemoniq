﻿/*
 *  Copyright 2009 Kriztian Jake Sta. Teresa
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
using Daemoniq.Core;
using Daemoniq.Core.Commands;
using Daemoniq.Framework;
using Daemoniq.Samples;
using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;

namespace Daemoniq.Tests.Core
{
    [TestFixture]
    public class ServiceControlHelperTests
    {
        private IConfiguration configuration;
        private CommandLineArguments commandLineArguments;
        private string assemblyLocation;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            assemblyLocation = typeof(DummyService).Assembly.Location;
            var mock = new Mock<IServiceLocator>();
            mock.Setup(s => s.GetInstance<IServiceInstance>("Dummy:SRHT"))
                .Returns(() => new DummyService());

            ServiceLocator.SetLocatorProvider(
                () => mock.Object);

            ICommand command = CommandFactory.CreateInstance(
                ConfigurationAction.Install);

            var serviceInfo = new ServiceInfo();
            serviceInfo.ServiceName = "Dummy:SRHT";
            serviceInfo.DisplayName = "Dummy-ServiceRecoveryHelperTest";

            commandLineArguments = new CommandLineArguments();
            commandLineArguments.AccountInfo = new AccountInfo(AccountType.LocalSystem);

            configuration = new Daemoniq.Framework.Configuration();
            configuration.Services.Add(serviceInfo);

            var installCommand = (command as InstallCommand);
            // ReSharper disable PossibleNullReferenceException
            installCommand.Execute(
                configuration,
                commandLineArguments,
                assemblyLocation);
            // ReSharper restore PossibleNullReferenceException
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            var command = CommandFactory.CreateInstance(ConfigurationAction.Uninstall);
            var uninstallCommand = command as UninstallCommand;
            // ReSharper disable PossibleNullReferenceException
            uninstallCommand.Execute(
                configuration,
                commandLineArguments,
                assemblyLocation);
            // ReSharper restore PossibleNullReferenceException
        }

        [Test]
        public void SetAndGetRecoveryOptions()
        {
            var expected = new ServiceRecoveryOptions();
            expected.FirstFailureAction = ServiceRecoveryAction.RunAProgram;
            expected.SecondFailureAction = ServiceRecoveryAction.RestartTheService;
            expected.SubsequentFailureActions = ServiceRecoveryAction.RestartTheComputer;
            expected.MinutesToRestartService = 5;
            expected.DaysToResetFailAcount = 2;
            expected.CommandToLaunchOnFailure = "Sample.exe";
            expected.RebootMessage = "OMGWTFBBQ!!!!";

            var serviceInfo = configuration.Services[0];
            ServiceControlHelper.SetServiceRecoveryOptions(serviceInfo.ServiceName, expected);
            var actual = ServiceControlHelper.GetServiceRecoveryOptions(serviceInfo.ServiceName);

            Assert.AreEqual(expected, actual);
        }
    }
}