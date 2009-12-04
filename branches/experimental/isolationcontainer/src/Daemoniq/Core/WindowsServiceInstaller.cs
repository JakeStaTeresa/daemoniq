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
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.ServiceProcess;
using Daemoniq.Framework;

namespace Daemoniq.Core
{
    class WindowsServiceInstaller : IDisposable
    {
        private readonly TransactedInstaller transactedInstaller;

        public WindowsServiceInstaller(
            IEnumerable<ServiceInfo> services,
            CommandLineArguments commandLineArguments,
            string assemblyPath)
        {
            LogHelper.EnterFunction(services, commandLineArguments, assemblyPath);
            ThrowHelper.ThrowArgumentNullIfNull(services, "services");
            ThrowHelper.ThrowArgumentNullIfNull(commandLineArguments, "commandLineArguments");
            ThrowHelper.ThrowArgumentNullIfNull(assemblyPath, "assemblyPath");

            transactedInstaller = createTransactedInstaller(
                services,
                commandLineArguments,
                assemblyPath);
            LogHelper.LeaveFunction();
        }

        public void Install()
        {
            transactedInstaller.Install(new Hashtable());
        }

        public void Uninstall()
        {
            transactedInstaller.Uninstall(null);
        }

        #region IDisposable Members

        public void Dispose()
        {
            transactedInstaller.Dispose();
        }

        #endregion

        private TransactedInstaller createTransactedInstaller(
            IEnumerable<ServiceInfo> services,
            CommandLineArguments commandLineArguments,
            string assemblyPath)
        {
            LogHelper.EnterFunction(services, commandLineArguments, assemblyPath);
            ThrowHelper.ThrowArgumentNullIfNull(services, "services");
            ThrowHelper.ThrowArgumentNullIfNull(commandLineArguments, "commandLineArguments");
            ThrowHelper.ThrowArgumentNullIfNull(assemblyPath, "assemblyPath");

            var returnValue = new TransactedInstaller();
            var installGroup = new Installer();
            foreach (var serviceInfo in services)
            {
                installGroup.Installers.Add(
                    createServiceInstaller(serviceInfo));
            }
            installGroup.Installers.Add(
                createServiceProcessInstaller(commandLineArguments));
            returnValue.Installers.Add(installGroup);            

            string assemblyPathParameter = string.Format("/assemblypath={0}",
                assemblyPath);
            var args = new List<string> { assemblyPathParameter };
            args.AddRange(
                getInstallContextArguments(
                    commandLineArguments));

            var installContext = new InstallContext("", args.ToArray());
            returnValue.Context = installContext;

            LogHelper.LeaveFunction();
            return returnValue;
        }
        
        private ServiceInstaller createServiceInstaller(ServiceInfo serviceInfo)
        {
            LogHelper.EnterFunction(serviceInfo);
            ThrowHelper.ThrowArgumentNullIfNull(serviceInfo, "serviceInfo");

            var serviceInstaller = new ServiceInstaller
            {
                ServiceName = serviceInfo.ServiceName,
                Description = serviceInfo.Description,
                DisplayName = serviceInfo.DisplayName,
                StartType = toServiceStartMode(serviceInfo.StartMode)
            };

            if (serviceInfo.ServicesDependedOn.Count > 0)
            {
                int numberOfServicesDependedOn = serviceInfo.ServicesDependedOn.Count;
                serviceInstaller.ServicesDependedOn = new string[numberOfServicesDependedOn];
                for (int i = 0; i < numberOfServicesDependedOn; i++)
                {
                    var serviceDependedOn = serviceInfo.ServicesDependedOn[i];
                    serviceInstaller.ServicesDependedOn[i] = serviceDependedOn;
                }
            }
            LogHelper.LeaveFunction();
            return serviceInstaller;
        }

        private ServiceProcessInstaller createServiceProcessInstaller(
            CommandLineArguments commandLineArguments)
        {
            LogHelper.EnterFunction(commandLineArguments);
            ThrowHelper.ThrowArgumentNullIfNull(commandLineArguments, "commandLineArguments");

            var accountInfo = commandLineArguments.AccountInfo;
            var serviceProcessInstaller = new ServiceProcessInstaller();

            serviceProcessInstaller.Account = toServiceAccount(accountInfo.AccountType);
            if (accountInfo.AccountType == AccountType.User &&
                !string.IsNullOrEmpty(accountInfo.Username) &&
                !string.IsNullOrEmpty(accountInfo.Password))
            {
                serviceProcessInstaller.Username = accountInfo.Username;
                serviceProcessInstaller.Password = accountInfo.Password;
            }

            LogHelper.LeaveFunction();
            return serviceProcessInstaller;
        }

        private IEnumerable<string> getInstallContextArguments
            (CommandLineArguments commandLineArguments)
        {
            LogHelper.EnterFunction(commandLineArguments);
            ThrowHelper.ThrowArgumentNullIfNull(commandLineArguments, "commandLineArguments");

            if (!string.IsNullOrEmpty(commandLineArguments.LogFile))
                yield return string.Format("/logfile={0}", commandLineArguments.LogFile);

            if (commandLineArguments.LogToConsole.HasValue)
                yield return string.Format("/logtoconsole={0}", commandLineArguments.LogToConsole.Value);

            if (commandLineArguments.ShowCallStack.HasValue)
                yield return string.Format("/showcallstack");

            LogHelper.LeaveFunction();
        }

        private ServiceAccount toServiceAccount(AccountType accountType)
        {
            LogHelper.EnterFunction(accountType);
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
            LogHelper.LeaveFunction();
            return serviceAccount;
        }

        private ServiceStartMode toServiceStartMode(StartMode startMode)
        {
            LogHelper.EnterFunction(startMode);

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

            LogHelper.LeaveFunction();
            return serviceStartMode;
        }        
    }
}