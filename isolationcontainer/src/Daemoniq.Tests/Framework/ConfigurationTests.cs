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
using NUnit.Framework;

namespace Daemoniq.Tests.Framework
{
    [TestFixture]
    public class ConfigurationTests
    {
        //[Test]
        //public void DefaultPropertyTest()
        //{
            //var configuration = new Daemoniq.Framework.Configuration();
            //var accountInfo = new AccountInfo(AccountType.User);
            //Assert.AreEqual(accountInfo.AccountType, configuration.AccountInfo.AccountType);
            //Assert.AreEqual(accountInfo.Username, configuration.AccountInfo.Username);
            //Assert.AreEqual(accountInfo.Password, configuration.AccountInfo.Password);
            //Assert.AreEqual(ConfigurationAction.Run, configuration.Action);
            //Assert.IsNull(configuration.Description);
            //Assert.IsNull(configuration.DisplayName);
            //Assert.IsNull(configuration.LogFile);
            //Assert.IsNull(configuration.LogToConsole);
            //Assert.IsNull(configuration.ServiceName);
            //Assert.IsNull(configuration.ShowCallStack);
            //Assert.AreEqual(0, configuration.ServicesDependedOn.Count);
            //Assert.AreEqual(StartMode.Manual, configuration.StartMode);
        //}

        [Test]
        public void DefaultPropertyTest()
        {
            var configuration = new Daemoniq.Framework.Configuration();
            Assert.IsNotNull(configuration.Services);
            Assert.AreEqual(0, configuration.Services.Count);
        }
    }
}
