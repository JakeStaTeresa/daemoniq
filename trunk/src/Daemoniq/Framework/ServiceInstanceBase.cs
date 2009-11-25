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
using System.ServiceProcess;

namespace Daemoniq.Framework
{
    public abstract class ServiceInstanceBase : IServiceInstance
    {
        private readonly ServiceRecoveryOptions recoveryOptions = 
            new ServiceRecoveryOptions();

        #region IService Members

        public abstract string ServiceName { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public virtual IEnumerable<string> ServicesDependedOn { get{ yield break; } }        
        
        public virtual bool CanHandleCustomCommand
        {
            get { return false; }
        }

        public virtual bool CanHandlePowerEvent
        {
            get { return false; }
        }

        public virtual bool CanHandleSessionChangeEvent
        {
            get { return false; }
        }

        public virtual bool CanPauseAndContinue
        {
            get { return false; }
        }

        public virtual bool CanShutdown
        {
            get { return false; }
        }

        public bool CanStop
        {
            get { return true; }
        }

        public abstract void OnStart();

        public abstract void OnStop();

        public virtual void OnPause()
        {
            throw new NotSupportedException();
        }

        public virtual void OnContinue()
        {
            throw new NotSupportedException();
        }

        public virtual void OnShutdown()
        {
            throw new NotSupportedException();
        }

        public virtual void OnCustomCommand(int command)
        {
            throw new NotSupportedException();
        }

        public virtual bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            throw new NotSupportedException();
        }

        public virtual void OnSessionChange(SessionChangeDescription changeDescription)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
