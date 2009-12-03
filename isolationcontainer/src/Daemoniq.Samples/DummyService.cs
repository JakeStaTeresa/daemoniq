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
using System.Threading;
using Daemoniq.Framework;

namespace Daemoniq.Samples
{
    class DummyService:ServiceInstanceBase
    {
        private readonly ManualResetEvent stopRequested = new ManualResetEvent(false);
        private const string serviceName = "DummyService";
        private const string displayName = "Dummy Service";
        private const string description = "This service was created using Daemoniq Framework.";

        public override string  ServiceName
        {
            get { return serviceName; }
        }

        public override string  DisplayName
        {
            get { return displayName; }
        }

        public override string  Description
        {
            get { return description; }
        }

        public override void OnStart()
        {
            var t = new Thread(threadProc);
            t.Start();
        }

        public override void OnStop()
        {
            stopRequested.Set();
        }

        private void threadProc()
        {
            stopRequested.WaitOne();
        }   
    }
}
