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
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;

using Daemoniq.Framework;

namespace Daemoniq.Core
{
    [RunInstaller(true)]
    class WindowsServiceInstaller : Installer
    {
        public WindowsServiceInstaller() { }
        
        public WindowsServiceInstaller(IConfiguration configuration)
        {
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");

            var serviceInstaller = new ServiceInstaller
                                       {
                                           ServiceName = configuration.ServiceName,
                                           Description = configuration.Description,
                                           DisplayName = configuration.DisplayName,
                                           StartType = toServiceStartMode(configuration.StartMode)
                                       };
            
            if(configuration.ServicesDependedOn.Count > 0)
            {
                int numberOfServicesDependedOn = configuration.ServicesDependedOn.Count;
                serviceInstaller.ServicesDependedOn = new string[numberOfServicesDependedOn];
                for (int i = 0; i < numberOfServicesDependedOn; i++)
                {
                    string serviceName = configuration.ServicesDependedOn[i];
                    serviceInstaller.ServicesDependedOn[i] = serviceName;
                }
            }

            var accountInfo = configuration.AccountInfo;
            var serviceProcessInstaller = new ServiceProcessInstaller();            

            serviceProcessInstaller.Account = toServiceAccount(accountInfo.AccountType);
            if (accountInfo.AccountType == AccountType.User &&
                !string.IsNullOrEmpty(accountInfo.Username) &&
                !string.IsNullOrEmpty(accountInfo.Password))
            {                
                serviceProcessInstaller.Username = accountInfo.Username;
                serviceProcessInstaller.Password = accountInfo.Password;
            }
            Installers.Add(serviceInstaller);
            Installers.Add(serviceProcessInstaller);
        }
       
        private ServiceAccount toServiceAccount(AccountType accountType)
        {
            ServiceAccount serviceAccount = default(ServiceAccount);
            switch (accountType)
            {
                case AccountType.LocalService:
                    serviceAccount = ServiceAccount.LocalService;
                    break;
                case AccountType.LocalSystem:
                    serviceAccount = ServiceAccount.LocalSystem;
                    break;
                case AccountType.NetworkService:
                    serviceAccount = ServiceAccount.NetworkService;
                    break;
                case AccountType.User:
                    serviceAccount = ServiceAccount.User;
                    break;
            }
            return serviceAccount;
        }

        private ServiceStartMode toServiceStartMode(StartMode startMode)
        {
            ServiceStartMode serviceStartMode = default(ServiceStartMode);
            switch (startMode)
            {
                case StartMode.Automatic:
                    serviceStartMode = ServiceStartMode.Automatic;
                    break;
                case StartMode.Disabled:
                    serviceStartMode = ServiceStartMode.Disabled;
                    break;
                case StartMode.Manual:
                    serviceStartMode = ServiceStartMode.Manual;
                    break;                
            }
            return serviceStartMode;
        }
    }
}