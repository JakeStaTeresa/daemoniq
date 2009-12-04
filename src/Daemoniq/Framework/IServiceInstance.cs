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

namespace Daemoniq.Framework
{
    public interface IServiceInstance
    {
        bool CanHandleCustomCommand { get; }        
        bool CanHandlePowerEvent { get; }
        bool CanHandleSessionChangeEvent { get; }
        bool CanPauseAndContinue { get; }
        bool CanShutdown { get; }
        bool CanStop { get; }

        void OnStart();
        void OnStop();
        void OnPause();
        void OnContinue();
        void OnShutdown();
        void OnCustomCommand(int command);
        bool OnPowerEvent(PowerBroadcastStatus powerStatus);
        void OnSessionChange(SessionChangeDescription changeDescription);
    }
}
