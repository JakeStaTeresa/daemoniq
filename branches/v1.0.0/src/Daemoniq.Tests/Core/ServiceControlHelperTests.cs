/*
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
using NUnit.Framework;

namespace Daemoniq.Tests.Core
{
    [TestFixture]
    public class ServiceControlHelperTests
    {
        [Test]
        public void SetAndGetRecoveryOptions()
        {            
            ICommand installCommand = CommandFactory.CreateInstance(ConfigurationAction.Install);
            DummyService dummyService = new DummyService();

            var configuration = new Daemoniq.Framework.Configuration();
            configuration.ServiceName = dummyService.ServiceName + ":SRHT";
            configuration.DisplayName = dummyService.DisplayName + "-ServiceRecoveryHelperTest";
            configuration.AccountInfo = new AccountInfo(AccountType.LocalSystem);                          
            installCommand.Execute(configuration, new DummyService());
        
            var expected = new ServiceRecoveryOptions();
            expected.FirstFailureAction = ServiceRecoveryAction.RunAProgram;
            expected.SecondFailureAction = ServiceRecoveryAction.RestartTheService;
            expected.SubsequentFailureActions = ServiceRecoveryAction.RestartTheComputer;
            expected.MinutesToRestartService = 5;
            expected.DaysToResetFailAcount = 2;
            expected.CommandToLaunchOnFailure = "Sample.exe";
            expected.RebootMessage = "OMGWTFBBQ!!!!";
            
            ServiceControlHelper.SetServiceRecoveryOptions(configuration.ServiceName, expected);            
            var actual = ServiceControlHelper.GetServiceRecoveryOptions(configuration.ServiceName);

            Assert.AreEqual(expected, actual);

            ICommand uninstallCommand = CommandFactory.CreateInstance(ConfigurationAction.Uninstall);            
            uninstallCommand.Execute(configuration, dummyService);
        }
    }
}