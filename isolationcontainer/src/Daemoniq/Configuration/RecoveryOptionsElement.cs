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
    public class RecoveryOptionsElement :
        ConfigurationElement
    {
        public RecoveryOptionsElement()
        {
            FirstFailureAction = ServiceRecoveryAction.TakeNoAction;
            SecondFailureAction = ServiceRecoveryAction.TakeNoAction;
            SubsequentFailureActions = ServiceRecoveryAction.TakeNoAction;
            DaysToResetFailAcount = 0;
            MinutesToRestartService = 1;
        }

        [ConfigurationProperty("firstFailureAction")]
        public ServiceRecoveryAction FirstFailureAction
        {
            get { return (ServiceRecoveryAction)(base["firstFailureAction"]); }
            set { base["firstFailureAction"] = value; }
        }

        [ConfigurationProperty("secondFailureAction")]
        public ServiceRecoveryAction SecondFailureAction
        {
            get { return (ServiceRecoveryAction)(base["secondFailureAction"]); }
            set { base["secondFailureAction"] = value; }
        }

        [ConfigurationProperty("subsequentFailureAction")]
        public ServiceRecoveryAction SubsequentFailureActions
        {
            get { return (ServiceRecoveryAction)(base["subsequentFailureAction"]); }
            set { base["subsequentFailureAction"] = value; }
        }

        [ConfigurationProperty("daysToResetFailAcount")]
        public int DaysToResetFailAcount
        {
            get { return (int)(base["daysToResetFailAcount"]); }
            set { base["daysToResetFailAcount"] = value; }
        }

        [ConfigurationProperty("minutesToRestartService")]
        public int MinutesToRestartService
        {
            get { return (int)(base["minutesToRestartService"]); }
            set { base["minutesToRestartService"] = value; }
        }

        [ConfigurationProperty("rebootMessage")]
        public string RebootMessage
        {
            get { return (string)(base["rebootMessage"]); }
            set { base["rebootMessage"] = value; }
        }

        [ConfigurationProperty("commandToLaunchOnFailure")]
        public string CommandToLaunchOnFailure
        {
            get { return (string)(base["commandToLaunchOnFailure"]); }
            set { base["commandToLaunchOnFailure"] = value; }
        }       
    }
}
