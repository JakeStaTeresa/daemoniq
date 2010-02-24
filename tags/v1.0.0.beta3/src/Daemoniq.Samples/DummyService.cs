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
using System.Diagnostics;
using System.Threading;
using Daemoniq.Framework;

namespace Daemoniq.Samples
{
    public class DummyService:ServiceInstanceBase
    {
        private readonly ManualResetEvent stopRequested = new ManualResetEvent(false);        

        public override void OnStart()
        {
            Trace.WriteLine("Starting instance : " + GetHashCode());
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
