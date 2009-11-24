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
    public class AccountInfoTests
    {
        [Test]
        public void ConstructUsingAccountTypeUser()
        {
            var accountInfo = new AccountInfo(AccountType.User);
            Assert.AreEqual(AccountType.User, accountInfo.AccountType);
            Assert.IsNull(accountInfo.Username);
            Assert.IsNull(accountInfo.Password);
        }

        [Test]
        public void ConstructUsingAccountTypeLocalService()
        {
            var accountInfo = new AccountInfo(AccountType.LocalService);
            Assert.AreEqual(AccountType.LocalService, accountInfo.AccountType);
            Assert.IsNull(accountInfo.Username);
            Assert.IsNull(accountInfo.Password);
        }

        [Test]
        public void ConstructUsingAccountTypeLocalSystem()
        {
            var accountInfo = new AccountInfo(AccountType.LocalSystem);
            Assert.AreEqual(AccountType.LocalSystem, accountInfo.AccountType);
            Assert.IsNull(accountInfo.Username);
            Assert.IsNull(accountInfo.Password);
        }

        [Test]
        public void ConstructUsingAccountTypeNetworkService()
        {
            var accountInfo = new AccountInfo(AccountType.NetworkService);
            Assert.AreEqual(AccountType.NetworkService, accountInfo.AccountType);
            Assert.IsNull(accountInfo.Username);
            Assert.IsNull(accountInfo.Password);
        }

        [Test]
        public void ConstructUsingUsernameAndPassword()
        {
            string username = "Jake.StaTeresa";
            string password = "OMGWTFBBQ!!!!";
            var accountInfo = new AccountInfo(username, password);
            Assert.AreEqual(AccountType.User, accountInfo.AccountType);
            Assert.AreEqual(username, accountInfo.Username);
            Assert.AreEqual(password, accountInfo.Password);            
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WillThrowArgumentNullWhenUsernameIsNull()
        {
            new AccountInfo(null, "");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WillThrowArgumentNullWhenPasswordIsNull()
        {
            new AccountInfo("Jake.StaTeresa", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WillThrowArgumentOutOfRangeWhenUsernameIsBlank()
        {
            new AccountInfo("", "OMGWTFBBQ!!!");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WillThrowArgumentOutOfRangeWhenPasswordIsBlank()
        {
            new AccountInfo("Jake.StaTeresa", "");
        }



    }
}
