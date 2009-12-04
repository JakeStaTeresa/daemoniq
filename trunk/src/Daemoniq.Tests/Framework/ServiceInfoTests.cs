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
using NUnit.Framework;

namespace Daemoniq.Tests.Framework
{
    [TestFixture]
    public class ServiceInfoTests
    {
        [Test]
        public void PropertyTests()
        {
            var serviceInfo = new ServiceInfo();
            Assert.IsNull(serviceInfo.Id); 
            Assert.IsNull(serviceInfo.ServiceName);
            Assert.IsNull(serviceInfo.DisplayName);
            Assert.IsNull(serviceInfo.Description);
            Assert.IsNotNull(serviceInfo.RecoveryOptions);
            Assert.AreEqual(StartMode.Manual, serviceInfo.StartMode);
            Assert.IsNotNull(serviceInfo.ServicesDependedOn);
            Assert.IsEmpty(serviceInfo.ServicesDependedOn);
            
            serviceInfo.Id = "DummyId";
            serviceInfo.ServiceName = "DummyServiceName";
            serviceInfo.DisplayName = "DummyDisplayName";
            serviceInfo.Description = "DummyDescription";
            var recoveryOptions = new ServiceRecoveryOptions();
            serviceInfo.RecoveryOptions = recoveryOptions;
            serviceInfo.StartMode = StartMode.Automatic;
            serviceInfo.ServicesDependedOn.Add(KnownServices.MsHttp);

            Assert.AreEqual("DummyId", serviceInfo.Id);
            Assert.AreEqual("DummyServiceName", serviceInfo.ServiceName);
            Assert.AreEqual("DummyDisplayName", serviceInfo.DisplayName);
            Assert.AreEqual("DummyDescription", serviceInfo.Description);
            Assert.AreEqual(recoveryOptions, serviceInfo.RecoveryOptions);
            Assert.AreEqual(StartMode.Automatic, serviceInfo.StartMode);
            Assert.Contains(KnownServices.MsHttp, serviceInfo.ServicesDependedOn);
        }
    }
}
