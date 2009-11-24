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
using System.Collections.Generic;

namespace Daemoniq.Framework
{
    public class Configuration:IConfiguration
    {
        private readonly List<string> servicesDependedOn;

        public Configuration()
        {            
            StartMode = StartMode.Manual;
            Action = ConfigurationAction.Run;
            AccountInfo = new AccountInfo(AccountType.User);
            servicesDependedOn = new List<string>();
        }

        #region IConfiguration Members

        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public List<string> ServicesDependedOn { get { return servicesDependedOn; } }
        public StartMode StartMode { get; set; }
        public AccountInfo AccountInfo { get; set; }
        public ConfigurationAction Action { get; set; }
        public ServiceRecoveryOptions RecoveryOptions { get; set; }        
        public string LogFile { get; set; }
        public bool? LogToConsole { get; set; }
        public bool? ShowCallStack { get; set; }
        
        #endregion        
    }
}