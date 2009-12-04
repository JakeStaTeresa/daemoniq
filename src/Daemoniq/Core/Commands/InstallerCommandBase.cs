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
using System.Collections.Generic;
using System.Linq;
using Daemoniq.Framework;
using Microsoft.Practices.ServiceLocation;

namespace Daemoniq.Core.Commands
{
    abstract class InstallerCommandBase : ICommand        
    {
        public void Install(
            IConfiguration configuration,
            CommandLineArguments commandLineArguments,
            string assemblyPath)
        {
            performOperation(
                configuration,
                commandLineArguments,
                assemblyPath,
                serviceInfo => ServiceControlHelper.IsServiceInstalled(serviceInfo.ServiceName),
                serviceInfo => string.Format("Service '{0}' is already installed.", serviceInfo.DisplayName),
                "There are  no serices to install.  Skipping install operation.",
                displayNames => string.Format("Installing services '{0}'...", displayNames),
                installer => installer.Install(),
                displayNames => string.Format("Services '{0}' successfully installed.", displayNames),
                services =>
                    {
                        foreach (var serviceInfo in services)
                        {
                            ServiceControlHelper.SetServiceRecoveryOptions(
                                serviceInfo.ServiceName,
                                serviceInfo.RecoveryOptions);
                            if (commandLineArguments.AllowServiceToInteractWithDesktop)
                            {
                                ServiceControlHelper.AllowServiceToInteractWithDesktop(serviceInfo.ServiceName);
                            }
                        }
                    });
        }

        public void Uninstall(
            IConfiguration configuration,
            CommandLineArguments commandLineArguments,
            string assemblyPath)
        {
            performOperation(
                configuration,
                commandLineArguments,
                assemblyPath,
                serviceInfo => !ServiceControlHelper.IsServiceInstalled(serviceInfo.ServiceName),
                serviceInfo => string.Format("Service '{0}' is not yet installed.", serviceInfo.DisplayName),
                "There are  no serices to uninstall.  Skipping uninstall operation.",
                displayNames => string.Format("Uninstalling services '{0}'...", displayNames),
                installer => installer.Uninstall(),
                displayNames => string.Format("Services '{0}' successfully uninstalled.", displayNames),
                services => { });
        }

        public abstract void Execute(
            IConfiguration configuration,
            CommandLineArguments commandLineArguments); 

        private void performOperation(
            IConfiguration configuration,
            CommandLineArguments commandLineArguments,
            string assemblyPath,
            Predicate<ServiceInfo> exclusionFilter,
            Converter<ServiceInfo, string> exclusionMessageConverter,
            string cancelMessage,
            Converter<string, string> preOperationMessageConverter,
            Action<WindowsServiceInstaller> operationAction,
            Converter<string, string> postOperationMessageConverter,
            Action<IEnumerable<ServiceInfo>> postOperationAction)
        {
            LogHelper.EnterFunction(configuration, 
                commandLineArguments,
                assemblyPath,
                exclusionFilter,
                exclusionMessageConverter,
                preOperationMessageConverter,
                cancelMessage,
                operationAction,
                postOperationMessageConverter,
                postOperationAction);
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");
            ThrowHelper.ThrowArgumentNullIfNull(commandLineArguments, "commandLineArguments");
            ThrowHelper.ThrowArgumentNullIfNull(assemblyPath, "assemblyPath");
            ThrowHelper.ThrowArgumentNullIfNull(exclusionFilter, "exclusionFilter");
            ThrowHelper.ThrowArgumentNullIfNull(exclusionMessageConverter, "exclusionMessageConverter");
            ThrowHelper.ThrowArgumentNullIfNull(cancelMessage, "cancelMessage");
            ThrowHelper.ThrowArgumentNullIfNull(preOperationMessageConverter, "preOperationMessageConverter");
            ThrowHelper.ThrowArgumentNullIfNull(operationAction, "operationAction");
            ThrowHelper.ThrowArgumentNullIfNull(postOperationMessageConverter, "postOperationMessageConverter");
            ThrowHelper.ThrowArgumentNullIfNull(postOperationAction, "postOperationAction");

            var services =
                getServicesToPerformActionOn(configuration,
                                              exclusionFilter,
                                              exclusionMessageConverter);

            if (services.Count == 0)
            {
                LogHelper.WriteLine(cancelMessage);
                Console.WriteLine(cancelMessage);
                return;
            }

            checkServicesInContainer(services);
            
            try
            {
                string displayNames =
                    string.Join("','", 
                        Array.ConvertAll(services.ToArray(),
                                         s => s.DisplayName));
                string preOperationMessage = preOperationMessageConverter(displayNames);
                LogHelper.WriteLine(preOperationMessage);
                Console.WriteLine(preOperationMessage);
                using(var installer = new WindowsServiceInstaller(
                    services,
                    commandLineArguments,
                    assemblyPath))
                {
                    operationAction(installer);
                }
                string postOperationMessage = postOperationMessageConverter(displayNames);
                LogHelper.WriteLine(postOperationMessage);
                Console.WriteLine(postOperationMessage);

                postOperationAction(services);
            }
            catch (Exception e)
            {                    
                LogHelper.Error(e);
                throw;
            }
            LogHelper.LeaveFunction();
        }

        private IList<ServiceInfo> getServicesToPerformActionOn(
            IConfiguration configuration,
            Predicate<ServiceInfo> predicate,
            Converter<ServiceInfo, string> converter)
        {
            var servicesToPerformActionsOn = new List<ServiceInfo>();
            foreach (var serviceInfo in configuration.Services)
            {
                if (predicate(serviceInfo))
                {
                    string message = converter(serviceInfo);
                    LogHelper.WriteLine(message);
                    Console.WriteLine(message);
                    continue;
                }

                servicesToPerformActionsOn.Add(serviceInfo);
            }
            return servicesToPerformActionsOn.AsReadOnly();
        }

        private void checkServicesInContainer(
            IEnumerable<ServiceInfo> services)
        {
            var serviceLocator = default(IServiceLocator);
            try
            {
                serviceLocator = ServiceLocator.Current;
            }
            catch (Exception e)
            {
                LogHelper.Error(e);
            }

            if (serviceLocator == null)
            {
                throw new InvalidOperationException("An error occured while getting service locator.");
            }

            foreach (var serviceInfo in services)
            {
                var serviceInstance =
                    serviceLocator.GetInstance<IServiceInstance>(serviceInfo.Id);
                if (serviceInstance == null)
                {
                    throw new InvalidOperationException(
                        string.Format("An error located while resolving service instance '{0}'. ", serviceInfo.ServiceName));
                }
            }
        }
    }
}