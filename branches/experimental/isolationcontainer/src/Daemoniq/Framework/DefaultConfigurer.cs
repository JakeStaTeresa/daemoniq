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
using System.Configuration;
using Daemoniq.Configuration;

namespace Daemoniq.Framework
{
    public class DefaultConfigurer :IConfigurer
    {
        #region IConfigurer<TService> Members

        public void Configure(IConfiguration configuration)
        {            
            System.Configuration.Configuration exeConfiguration =
                ConfigurationManager.OpenExeConfiguration(
                    ConfigurationUserLevel.None);
            DaemoniqConfigurationSection configurationSection = null;
            ConfigurationSectionCollection sections = exeConfiguration.Sections;
            foreach (ConfigurationSection section in sections)
            {
                if (section is DaemoniqConfigurationSection)
                {
                    configurationSection = section
                        as DaemoniqConfigurationSection;
                    break;
                }
            }

            if (configurationSection != null)
            {
                configuration.StartMode = configurationSection.ServiceStartMode;

                if (configurationSection.ServicesDependedOn != null &&
                    configurationSection.ServicesDependedOn.Count > 0)
                {
                    List<string> servicesDependedOn = new List<string>();
                    foreach (var service in configurationSection.ServicesDependedOn)
                    {
                        servicesDependedOn.Add(service.Name);
                    }
                    configuration.ServicesDependedOn.AddRange(servicesDependedOn);
                }

                if (configurationSection.RecoveryOptions != null)
                {
                    configuration.RecoveryOptions.FirstFailureAction =
                        configurationSection.RecoveryOptions.FirstFailureAction;
                    configuration.RecoveryOptions.SecondFailureAction =
                        configurationSection.RecoveryOptions.SecondFailureAction;
                    configuration.RecoveryOptions.SubsequentFailureActions =
                        configurationSection.RecoveryOptions.SubsequentFailureActions;
                    configuration.RecoveryOptions.DaysToResetFailAcount =
                        configurationSection.RecoveryOptions.DaysToResetFailAcount;
                    configuration.RecoveryOptions.MinutesToRestartService =
                        configurationSection.RecoveryOptions.MinutesToRestartService;
                    configuration.RecoveryOptions.RebootMessage =
                        configurationSection.RecoveryOptions.RebootMessage;
                    configuration.RecoveryOptions.CommandToLaunchOnFailure =
                        configurationSection.RecoveryOptions.CommandToLaunchOnFailure;                    
                }
            }
        }

        #endregion          
    }
}
