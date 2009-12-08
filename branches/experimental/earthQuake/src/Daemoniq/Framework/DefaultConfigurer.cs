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
using System.Configuration;
using Daemoniq.Configuration;

namespace Daemoniq.Framework
{
    public class DefaultConfigurer :IConfigurer
    {
        #region IConfigurer Members

        public IConfiguration Configure()
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

            IConfiguration configuration = default(IConfiguration);                
            if (configurationSection != null)
            {
                configuration = new Configuration();
                
                foreach(var serviceElement in configurationSection.Services)
                {
                    configuration.Services.Add(
                        ServiceInfo.FromConfiguration(serviceElement));
                }
            }
            return configuration;
        }

        #endregion          
    }
}