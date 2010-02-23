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

using Common.Logging;
using Daemoniq.Framework;

namespace Daemoniq.Core
{
    class WindowsServiceBase:ServiceBase        
    {
        private static ILog log = LogManager.GetCurrentClassLogger();
        private readonly IServiceInstance serviceInstance;

        public WindowsServiceBase(string serviceName,
            IServiceInstance serviceInstance)
        {
            ThrowHelper.ThrowArgumentNullIfNull(serviceInstance, "serviceInstance");
            ServiceName = serviceName;
            
            this.serviceInstance = serviceInstance;            
        }                        

        protected override void OnStart(string[] args)
        {
            log.Debug(m => m("Staring service '{0}'...", ServiceName));           
            serviceInstance.OnStart();
            log.Debug(m => m("Service '{0}' started.", ServiceName));                       
        }

        protected override void OnStop()
        {
            log.Debug(m => m("Stopping service '{0}'...", ServiceName));
            if (serviceInstance.CanStop)
                serviceInstance.OnStop();
            log.Debug(m => m("Service '{0}' stopped.", ServiceName));                      
        }

        protected override void OnPause()
        {
            log.Debug(m => m("Pausing service '{0}'...", ServiceName));
            if (serviceInstance.CanPauseAndContinue)
                serviceInstance.OnPause();
            log.Debug(m => m("Service '{0}' paused.", ServiceName));              
        }

        protected override void OnContinue()
        {
            log.Debug(m => m("Continuing service '{0}'...", ServiceName));
            if (serviceInstance.CanPauseAndContinue)
                serviceInstance.OnContinue();
            log.Debug(m => m("Service '{0}' continued.", ServiceName));              
        }

        protected override void OnShutdown()
        {
            log.Debug(m => m("Shutting down service '{0}'...", ServiceName));
            if (serviceInstance.CanShutdown)
                serviceInstance.OnShutdown();
            log.Debug(m => m("Service '{0}' shut down.", ServiceName));             
        }

        protected override void OnCustomCommand(int command)
        {
            log.Debug(m => m("Executing custom command '{0}' on service '{1}'...", command, ServiceName));
            if (serviceInstance.CanHandleCustomCommand)
                serviceInstance.OnCustomCommand(command);
            log.Debug(m => m("Done executing custom command '{0}' on service '{1}'.", command, ServiceName));            
        }

        protected override bool OnPowerEvent(
            PowerBroadcastStatus powerStatus)
        {
            log.Debug(m => m("Handling power event '{0}' on service '{1}'...", powerStatus, ServiceName));
            bool returnValue = false;
            if (serviceInstance.CanHandlePowerEvent)
                returnValue = serviceInstance.OnPowerEvent(powerStatus);
            log.Debug(m => m("Done handling power event '{0}' on service '{1}'.", powerStatus, ServiceName));
            return returnValue;
        }

        protected override void OnSessionChange(
            SessionChangeDescription changeDescription)
        {
            log.Debug(m => m("Handling session changed event on service '{0}'...", ServiceName));
            if (serviceInstance.CanHandleSessionChangeEvent)
                serviceInstance.OnSessionChange(changeDescription);
            log.Debug(m => m("Done handling session changed event on service '{0}'.", ServiceName));
            
        }
    }
}