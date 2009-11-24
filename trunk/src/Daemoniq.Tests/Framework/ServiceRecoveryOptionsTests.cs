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
using System;
using Daemoniq.Framework;
using NUnit.Framework;

namespace Daemoniq.Tests.Framework
{
    [TestFixture]
    public class ServiceRecoveryOptionsTests
    {
        [Test]
        public  void DefaultPropertyTests()
        {
            var serviceRecoveryOptions = new ServiceRecoveryOptions();
            Assert.IsNull(serviceRecoveryOptions.CommandToLaunchOnFailure);
            Assert.AreEqual(0,
                serviceRecoveryOptions.DaysToResetFailAcount);
            Assert.AreEqual(ServiceRecoveryAction.TakeNoAction,
                serviceRecoveryOptions.FirstFailureAction);
            Assert.AreEqual(1,
                serviceRecoveryOptions.MinutesToRestartService);
            Assert.IsNull(serviceRecoveryOptions.RebootMessage);
            Assert.AreEqual(ServiceRecoveryAction.TakeNoAction,
                serviceRecoveryOptions.SecondFailureAction);
            Assert.AreEqual(ServiceRecoveryAction.TakeNoAction,
                serviceRecoveryOptions.SubsequentFailureActions);
        }

        [Test]
        public void RebootMessageTest()
        {
            var serviceRecoveryOptions = new ServiceRecoveryOptions();
            serviceRecoveryOptions.SubsequentFailureActions = ServiceRecoveryAction.RestartTheComputer;
            serviceRecoveryOptions.RebootMessage = "This is a sample reboot message";
            serviceRecoveryOptions.Validate();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WillThrowInvalidOperationWhenRebootMessageIsDefinedAndFailureActionRestartTheComputerIsNotDefined()
        {
            var serviceRecoveryOptions = new ServiceRecoveryOptions();
            serviceRecoveryOptions.RebootMessage = "This is a sample reboot message";
            serviceRecoveryOptions.Validate();
        }

        [Test]
        public void CommandToLaunchOnFailureTest()
        {
            var serviceRecoveryOptions = new ServiceRecoveryOptions();
            serviceRecoveryOptions.SecondFailureAction = ServiceRecoveryAction.RunAProgram;
            serviceRecoveryOptions.CommandToLaunchOnFailure = "Sample.exe";
            serviceRecoveryOptions.Validate();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WillThrowInvalidOperationWhenCommandToLaunchOnFailureIsDefinedAndFailureActionRunAProgramIsNotDefined()
        {
            var serviceRecoveryOptions = new ServiceRecoveryOptions();
            serviceRecoveryOptions.CommandToLaunchOnFailure = "Sample.exe";
            serviceRecoveryOptions.Validate();
        }

        [Test]
        public void MinutesToRestartServiceTest()
        {
            var serviceRecoveryOptions = new ServiceRecoveryOptions();
            serviceRecoveryOptions.FirstFailureAction = ServiceRecoveryAction.RestartTheService;
            serviceRecoveryOptions.MinutesToRestartService = 100;
            serviceRecoveryOptions.Validate();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WillThrowInvalidOperationWhenMinutesToRestartServiceIsDefinedAndFailureActionRunAProgramIsNotDefined()
        {
            var serviceRecoveryOptions = new ServiceRecoveryOptions();
            serviceRecoveryOptions.MinutesToRestartService = 100;
            serviceRecoveryOptions.Validate();
        }  
    }
}
