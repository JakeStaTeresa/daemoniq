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

namespace Daemoniq.Core
{
    public class WindowsServiceInstaller : IDisposable
    {
        private readonly TransactedInstaller transactedInstaller;

        public WindowsServiceInstaller(
            IEnumerable<ServiceInfo> services,
            IConfiguration configuration,
            string assemblyPath)
        {
            LogHelper.EnterFunction(services, configuration, assemblyPath);
            ThrowHelper.ThrowArgumentNullIfNull(services, "services");
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");
            ThrowHelper.ThrowArgumentNullIfNull(assemblyPath, "assemblyPath");

            transactedInstaller = createTransactedInstaller(
                services,
                configuration,
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
            IConfiguration configuration,
            string assemblyPath)
        {
            LogHelper.EnterFunction(services, configuration, assemblyPath);
            ThrowHelper.ThrowArgumentNullIfNull(services, "services");
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");
            ThrowHelper.ThrowArgumentNullIfNull(assemblyPath, "assemblyPath");

            var returnValue = new TransactedInstaller();
            var installGroup = new Installer();
            foreach (var serviceInfo in services)
            {
                installGroup.Installers.Add(
                    createServiceInstaller(serviceInfo));
            }
            installGroup.Installers.Add(
                createServiceProcessInstaller(configuration));
            returnValue.Installers.Add(installGroup);            

            string assemblyPathParameter = string.Format("/assemblypath={0}",
                assemblyPath);
            var args = new List<string> { assemblyPathParameter };
            args.AddRange(
                getInstallContextArguments(
                    configuration));

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
                StartType = serviceInfo.StartMode
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
            IConfiguration configuration)
        {
            LogHelper.EnterFunction(configuration);
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");

            var accountInfo = configuration.AccountInfo;
            var serviceProcessInstaller = new ServiceProcessInstaller();

            serviceProcessInstaller.Account = accountInfo.AccountType;
            if (accountInfo.AccountType == ServiceAccount.User &&
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
            (IConfiguration commandLineArguments)
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
    }
}