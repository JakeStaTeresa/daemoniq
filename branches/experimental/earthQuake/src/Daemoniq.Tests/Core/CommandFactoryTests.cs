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
using Daemoniq.Framework;
using Daemoniq.Core.Commands;
using NUnit.Framework;

namespace Daemoniq.Tests.Core
{
    [TestFixture]
    public class CommandFactoryTests
    {
        [Test]
        public void CreateConsoleCommandTest()
        {
            var command = createCommand(ConfigurationAction.Console);
            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(typeof(ConsoleCommand), command);
        }

        [Test]
        public void CreateInstallCommandTest()
        {
            var command = createCommand(ConfigurationAction.Install);
            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(typeof(InstallCommand), command);
        }

        [Test]
        public void CreateUninstallCommandTest()
        {
            var command = createCommand(ConfigurationAction.Uninstall);
            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(typeof(UninstallCommand), command);
        }

        [Test]
        public void CreateRunCommandTest()
        {
            var command = createCommand(ConfigurationAction.Run);
            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(typeof(RunCommand), command);
        }

        private ICommand createCommand(ConfigurationAction action)
        {
            return CommandFactory.CreateInstance(action);
        }
    }
}