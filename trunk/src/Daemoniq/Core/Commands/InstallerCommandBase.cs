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
using Daemoniq.Framework;

namespace Daemoniq.Core.Commands
{
    abstract class InstallerCommandBase : ICommand        
    {
        public void Install(IConfiguration configuration,
            IServiceInstance serviceInstance)
        {
            LogHelper.EnterFunction(configuration, serviceInstance);
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");
            ThrowHelper.ThrowArgumentNullIfNull(serviceInstance, "serviceInstance");

            if (ServiceControlHelper.IsServiceInstalled(configuration.ServiceName))
            {                         
                LogHelper.WriteLine("Service '{0}' is already installed.", configuration.DisplayName);                
            }
            else
            {
                try
                {
                    LogHelper.WriteLine("Installing service '{0}'...", configuration.DisplayName);
                    var transactedInstaller = createTransactedInstaller(configuration, serviceInstance);
                    
                    transactedInstaller.Install(new Hashtable());
                    LogHelper.WriteLine("Service '{0}' successfully installed.", configuration.DisplayName);
                    ServiceControlHelper.SetServiceRecoveryOptions(
                        configuration.ServiceName,
                        configuration.RecoveryOptions);
                    
                }
                catch (Exception e)
                {                    
                    LogHelper.Error(e);
                }
            }
            LogHelper.LeaveFunction();
        }

        public void Uninstall(IConfiguration configuration,
            IServiceInstance serviceInstance)
        {
            LogHelper.EnterFunction(configuration, serviceInstance);
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");
            ThrowHelper.ThrowArgumentNullIfNull(serviceInstance, "serviceInstance");
            if (!ServiceControlHelper.IsServiceInstalled(configuration.ServiceName))
            {
                LogHelper.WriteLine("Service '{0}' is not yet installed.", configuration.DisplayName);
            }
            else
            {
                try
                {
                    LogHelper.WriteLine("Uninstalling service '{0}'...", configuration.DisplayName);
                    var transactedInstaller = createTransactedInstaller(configuration, serviceInstance);
                    transactedInstaller.Uninstall(null);
                    LogHelper.WriteLine("Service '{0}' successfully uninstalled.", configuration.DisplayName);
                }
                catch (Exception e)
                {
                    LogHelper.Error(e);
                }
            } 
            LogHelper.LeaveFunction();
        }        

        private TransactedInstaller createTransactedInstaller(IConfiguration configuration,
            IServiceInstance serviceInstance)
        {
            LogHelper.EnterFunction(configuration);
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");
            
            var transactedInstaller = new TransactedInstaller();
            var serviceInstaller = new WindowsServiceInstaller(configuration);
            transactedInstaller.Installers.Add(serviceInstaller);            

            string assemblyPath = string.Format("/assemblypath={0}",
                serviceInstance.GetType().Assembly.Location);
            var args = new List<string> { assemblyPath };
            args.AddRange(
                getInstallContextArguments(
                    configuration));

            var installContext = new InstallContext("", args.ToArray());
            transactedInstaller.Context = installContext;

            LogHelper.LeaveFunction();
            return transactedInstaller;
        }

        private IEnumerable<string> getInstallContextArguments
            (IConfiguration configuration)
        {            
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");
            
            if (!string.IsNullOrEmpty(configuration.LogFile))
                yield return string.Format("/logfile={0}", configuration.LogFile);

            if (configuration.LogToConsole.HasValue)
                yield return string.Format("/logtoconsole={0}", configuration.LogToConsole.Value);

            if (configuration.ShowCallStack.HasValue)
                yield return string.Format("/showcallstack");
        }

        #region ICommand Members

        public abstract void Execute(IConfiguration configuration,
            IServiceInstance serviceInstance);

        #endregion
    }
}
