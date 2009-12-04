﻿/*
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
using System.ServiceProcess;
using Daemoniq.Framework;
using Microsoft.Practices.ServiceLocation;

namespace Daemoniq.Core.Commands
{
    class RunCommand : InstallerCommandBase        
    {
        public override void Execute(
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

            var servicesToRun = new List<ServiceBase>();
            foreach (var serviceInfo in configuration.Services)
            {
                if (!ServiceControlHelper.IsServiceInstalled(serviceInfo.ServiceName))
                {
                    LogHelper.WriteLine("Service '{0}' is not yet installed.", serviceInfo.DisplayName);
                    Console.WriteLine("Service '{0}' is not yet installed.", serviceInfo.DisplayName);
                    continue;
                }

                var serviceInstance =
                    serviceLocator.GetInstance<IServiceInstance>(serviceInfo.ServiceName);
                if (serviceInstance == null)
                {
                    throw new InvalidOperationException(
                        string.Format("An error located while resolving service instance '{0}'. ", serviceInfo.ServiceName));
                }

                servicesToRun.Add(new WindowsServiceBase(
                    serviceInfo.ServiceName,
                    serviceInstance));
            }

            try
            {
                LogHelper.WriteLine("Starting service process...");
                ServiceBase.Run(servicesToRun.ToArray());
            }
            catch (Exception e)
            {             
                LogHelper.Error(e);                   
            }            
            LogHelper.LeaveFunction();
        }
    }
}