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
using System.Reflection;
using System.ServiceProcess;
using Daemoniq.Framework;
using Microsoft.Practices.ServiceLocation;

namespace Daemoniq.Core.Commands
{
    abstract class InstallerCommandBase : CommandBase        
    {
        public void Install(
            IConfiguration configuration,
            CommandLineArguments commandLineArguments)
        {
            LogHelper.EnterFunction(configuration, commandLineArguments);
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");
            ThrowHelper.ThrowArgumentNullIfNull(commandLineArguments, "commandLineArguments");

            var serviceLocator = ServiceLocator.Current;
            if (serviceLocator == null)
            {
                throw new InvalidOperationException("An error occured while getting service locator.");
            }

            var servicesToInstall = new List<ServiceInfo>();
            foreach (var serviceInfo in configuration.Services)
            {
                var serviceInstance =
                    serviceLocator.GetInstance<IServiceInstance>(serviceInfo.Id);
                if (serviceInstance == null)
                {
                    throw new InvalidOperationException(
                        string.Format("An error located while resolving service instance '{0}'. ", serviceInfo.ServiceName));
                }
            
                if (ServiceControlHelper.IsServiceInstalled(serviceInfo.ServiceName))
                {
                    LogHelper.WriteLine("Service '{0}' is already installed.", serviceInfo.DisplayName);
                    Console.WriteLine("Service '{0}' is already installed.", serviceInfo.DisplayName);
                    continue;
                }

                servicesToInstall.Add(serviceInfo);
            }

            if (servicesToInstall.Count == 0)
            {
                LogHelper.WriteLine("There are  no serices to install.  Skipping install operation.");
                Console.WriteLine("There are  no serices to install.  Skipping install operation.");
                return;
            }

            try
            {
                string displayNamesToInstall =
                    string.Join("','", 
                        Array.ConvertAll(servicesToInstall.ToArray(),
                                         s => s.DisplayName));
                LogHelper.WriteLine("Installing services '{0}'...", displayNamesToInstall);
                var transactedInstaller = createTransactedInstaller(
                    servicesToInstall, commandLineArguments);
                
                transactedInstaller.Install(new Hashtable());
                LogHelper.WriteLine("Service '{0}' successfully installed.", displayNamesToInstall);
                Console.WriteLine("Service '{0}' successfully installed.", displayNamesToInstall);

                foreach (var serviceInfo in servicesToInstall)
                {
                    ServiceControlHelper.SetServiceRecoveryOptions(
                        serviceInfo.ServiceName,
                        serviceInfo.RecoveryOptions);
                    if (serviceInfo.Interactive)
                    {
                        ServiceControlHelper.AllowServiceToInteractWithDesktop(serviceInfo.ServiceName);
                    }
                }
            }
            catch (Exception e)
            {                    
                LogHelper.Error(e);
            }
            LogHelper.LeaveFunction();
        }

        public void Uninstall(
            IConfiguration configuration,
            CommandLineArguments commandLineArguments)
        {
            LogHelper.EnterFunction(configuration, commandLineArguments);
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");
            ThrowHelper.ThrowArgumentNullIfNull(commandLineArguments, "commandLineArguments");

            var serviceLocator = ServiceLocator.Current;
            if (serviceLocator == null)
            {
                throw new InvalidOperationException("An error occured while getting service locator.");
            }

            var servicesToUninstall = new List<ServiceInfo>();
            foreach (var serviceInfo in configuration.Services)
            {
                var serviceInstance =
                    serviceLocator.GetInstance<IServiceInstance>(serviceInfo.Id);
                if (serviceInstance == null)
                {
                    throw new InvalidOperationException(
                        string.Format("An error located while resolving service instance '{0}'. ", serviceInfo.ServiceName));
                }                
            
                if (!ServiceControlHelper.IsServiceInstalled(serviceInfo.ServiceName))
                {
                    LogHelper.WriteLine("Service '{0}' is not yet installed.", serviceInfo.DisplayName);
                    Console.WriteLine("Service '{0}' is not yet installed.", serviceInfo.DisplayName);
                    continue;
                }

                servicesToUninstall.Add(serviceInfo);
            }

            if(servicesToUninstall.Count == 0)
            {
                LogHelper.WriteLine("There are  no serices to uninstall.  Skipping uninstall operation.");
                Console.WriteLine("There are  no serices to uninstall.  Skipping uninstall operation.");
                return;
            }

            try
            {
                string displayNamesToUninstall =
                    string.Join("','",
                        Array.ConvertAll(servicesToUninstall.ToArray(),
                                         s => s.DisplayName));
                LogHelper.WriteLine("Uninstalling services '{0}'...", displayNamesToUninstall);
                var transactedInstaller = createTransactedInstaller(
                    servicesToUninstall, commandLineArguments);

                transactedInstaller.Uninstall(null);
                LogHelper.WriteLine("Service '{0}' successfully uninstalled.", displayNamesToUninstall);
                Console.WriteLine("Service '{0}' successfully uninstalled.", displayNamesToUninstall);
            }
            catch (Exception e)
            {
                LogHelper.Error(e);
            }
            LogHelper.LeaveFunction();
        }        

        private TransactedInstaller createTransactedInstaller(
            IEnumerable<ServiceInfo> services,
            CommandLineArguments commandLineArguments)
        {
            LogHelper.EnterFunction(services);
            ThrowHelper.ThrowArgumentNullIfNull(services, "services");
            ThrowHelper.ThrowArgumentNullIfNull(commandLineArguments, "commandLineArguments");

            var transactedInstaller = new TransactedInstaller();
            var installGroup = new Installer();
            foreach (var serviceInfo in services)
            {
                installGroup.Installers.Add(
                    createServiceInstaller(serviceInfo));
            }
            installGroup.Installers.Add(
                createServiceProcessInstaller(commandLineArguments));
            transactedInstaller.Installers.Add(installGroup);

            string assemblyPath = string.Format("/assemblypath={0}",
                Assembly.GetEntryAssembly().Location);
            var args = new List<string> { assemblyPath };
            args.AddRange(
                getInstallContextArguments(
                    commandLineArguments));

            var installContext = new InstallContext("", args.ToArray());
            transactedInstaller.Context = installContext;

            LogHelper.LeaveFunction();
            return transactedInstaller;
        }

        private IEnumerable<string> getInstallContextArguments
            (CommandLineArguments commandLineArguments)
        {
            ThrowHelper.ThrowArgumentNullIfNull(commandLineArguments, "commandLineArguments");

            if (!string.IsNullOrEmpty(commandLineArguments.LogFile))
                yield return string.Format("/logfile={0}", commandLineArguments.LogFile);

            if (commandLineArguments.LogToConsole.HasValue)
                yield return string.Format("/logtoconsole={0}", commandLineArguments.LogToConsole.Value);

            if (commandLineArguments.ShowCallStack.HasValue)
                yield return string.Format("/showcallstack");
        }        

        private ServiceInstaller createServiceInstaller(ServiceInfo serviceInfo)
        {
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

            return serviceInstaller;
        }

        private ServiceProcessInstaller createServiceProcessInstaller(
            CommandLineArguments commandLineArguments)
        {
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
            return serviceProcessInstaller;
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
