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
using Daemoniq.Framework;
using Microsoft.Practices.ServiceLocation;

namespace Daemoniq.Core.Commands
{
    class ConsoleCommand : CommandBase
    {
        #region ICommand Members

        public override void Execute(
            IConfiguration configuration,
            CommandLineArguments commandLineArguments)
        {
            LogHelper.EnterFunction(configuration, commandLineArguments);
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");
            ThrowHelper.ThrowArgumentNullIfNull(commandLineArguments, "commandLineArguments");
 
            var serviceLocator = ServiceLocator.Current;
            if(serviceLocator == null)
            {
                throw new InvalidOperationException("An error occured while getting service locator.");
            }

            try
            {
                Console.Write("Starting service process...");
                
                var servicesStarted = new List<IServiceInstance>();
                foreach (var serviceElement in configuration.Services)
                {
                    var serviceInstance =
                        serviceLocator.GetInstance<IServiceInstance>(serviceElement.Id);
                    if(serviceInstance == null)
                    {
                        throw new InvalidOperationException(
                            string.Format("An error located while resolving service instance '{0}'. ", serviceElement.ServiceName));
                    }
                    serviceInstance.OnStart();
                    servicesStarted.Add(serviceInstance);
                }
                Console.WriteLine("Done.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);

                Console.WriteLine("Terminating service process...");
                foreach (var serviceInstance in servicesStarted)
                {
                    serviceInstance.OnStop();
                }
                Console.WriteLine("Done.");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured while running service process.");
                Console.WriteLine(e);
            }
            finally
            {
                LogHelper.LeaveFunction();
            }
        }

        #endregion
    }
}
