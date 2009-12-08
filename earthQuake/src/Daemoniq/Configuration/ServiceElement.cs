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
using Daemoniq.Framework;

namespace Daemoniq.Configuration
{
    public class ServiceElement :
       ConfigurationElement
    {
        public ServiceElement()
        {
            StartMode = StartMode.Manual;
        }

        [ConfigurationProperty("serviceName", IsRequired = true, IsKey = true)]
        public string ServiceName
        {
            get { return (string)(base["serviceName"]); }
            set { base["serviceName"] = value; }
        }

        [ConfigurationProperty("displayName")]
        public string DisplayName
        {
            get { return (string)(base["displayName"]); }
            set { base["displayName"] = value; }
        }

        [ConfigurationProperty("description")]
        public string Description
        {
            get { return (string)(base["description"]); }
            set { base["description"] = value; }
        }

        [ConfigurationProperty("serviceStartMode")]
        public StartMode StartMode
        {
            get { return (StartMode)(base["serviceStartMode"]); }
            set { base["serviceStartMode"] = value; }
        }

        [ConfigurationProperty("recoveryOptions")]
        public RecoveryOptionsElement RecoveryOptions
        {
            get { return (RecoveryOptionsElement)this["recoveryOptions"]; }
            set { this["recoveryOptions"] = value; }
        }

        [ConfigurationProperty("servicesDependedOn",
           IsDefaultCollection = true, IsRequired = false)]
        public ServiceDependedOnElementCollection ServicesDependedOn
        {
            get { return (ServiceDependedOnElementCollection)this["servicesDependedOn"]; }
        }
    }
}