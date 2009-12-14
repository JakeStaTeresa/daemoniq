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
using System.ServiceProcess;

namespace Daemoniq.Core
{    
    public class AccountInfo
    {
        public ServiceAccount AccountType { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public AccountInfo(ServiceAccount accountType)
        {
            AccountType = accountType;
        }

        public AccountInfo(string username, string password)
        {
            ThrowHelper.ThrowArgumentNullIfNull(username, "username");
            ThrowHelper.ThrowArgumentNullIfNull(password, "password");
            ThrowHelper.ThrowArgumentOutOfRangeIfEmpty(username, "username");
            ThrowHelper.ThrowArgumentOutOfRangeIfEmpty(password, "password");

            Username = username;
            Password = password;
            AccountType = ServiceAccount.User;
        }
    }
}