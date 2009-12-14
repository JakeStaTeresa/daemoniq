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
using System.Reflection;

namespace Daemoniq.Core.Commands
{
    public class InstallCommand:InstallerCommandBase
    {
        public override void Execute(
            IConfiguration configuration)
        {
            LogHelper.EnterFunction(configuration);
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");

            Assembly entryAssembly = Assembly.GetEntryAssembly();
            ThrowHelper.ThrowInvalidOperationExceptionIf(
                a => a == null, entryAssembly,
                "Unable to get entry assembly.");
            Execute(configuration, entryAssembly.Location);
            LogHelper.LeaveFunction();
        }

        public void Execute(
            IConfiguration configuration,
            string assemblyPath)
        {
            LogHelper.EnterFunction(configuration, assemblyPath);
            ThrowHelper.ThrowArgumentNullIfNull(configuration, "configuration");
            ThrowHelper.ThrowArgumentNullIfNull(assemblyPath, "assemblyPath");

            Install(configuration, assemblyPath);
            LogHelper.LeaveFunction();
        }
    }
}
        