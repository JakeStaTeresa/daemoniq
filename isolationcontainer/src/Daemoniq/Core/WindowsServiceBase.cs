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
using System.ServiceProcess;

using Daemoniq.Framework;

namespace Daemoniq.Core
{
    class WindowsServiceBase:ServiceBase        
    {
        private readonly IServiceInstance serviceInstance;

        public WindowsServiceBase(IServiceInstance serviceInstance)
        {
            ThrowHelper.ThrowArgumentNullIfNull(serviceInstance, "serviceInstance");

            this.serviceInstance = serviceInstance;            
        }                        

        protected override void OnStart(string[] args)
        {
            LogHelper.EnterFunction(args);             
            serviceInstance.OnStart();
            LogHelper.LeaveFunction();
        }

        protected override void OnStop()
        {
            LogHelper.EnterFunction();
            if (serviceInstance.CanStop)
                serviceInstance.OnStop();
            LogHelper.LeaveFunction();
        }

        protected override void OnPause()
        {
            LogHelper.EnterFunction();
            if (serviceInstance.CanPauseAndContinue)
                serviceInstance.OnPause();
            LogHelper.LeaveFunction();
        }

        protected override void OnContinue()
        {
            LogHelper.EnterFunction();
            if (serviceInstance.CanPauseAndContinue)
                serviceInstance.OnContinue();
            LogHelper.LeaveFunction();
        }

        protected override void OnShutdown()
        {
            LogHelper.EnterFunction();
            if (serviceInstance.CanShutdown)
                serviceInstance.OnShutdown();
            LogHelper.LeaveFunction();
        }

        protected override void OnCustomCommand(int command)
        {
            LogHelper.EnterFunction();
            if (serviceInstance.CanHandleCustomCommand)
                serviceInstance.OnCustomCommand(command);
            LogHelper.LeaveFunction();
        }

        protected override bool OnPowerEvent(
            PowerBroadcastStatus powerStatus)
        {
            LogHelper.EnterFunction();
            if (serviceInstance.CanHandlePowerEvent)
                return serviceInstance.OnPowerEvent(powerStatus);
            LogHelper.LeaveFunction();
            return false;
        }

        protected override void OnSessionChange(
            SessionChangeDescription changeDescription)
        {
            LogHelper.EnterFunction();
            if (serviceInstance.CanHandleSessionChangeEvent)
                serviceInstance.OnSessionChange(changeDescription);
            LogHelper.LeaveFunction();
        }
    }
}