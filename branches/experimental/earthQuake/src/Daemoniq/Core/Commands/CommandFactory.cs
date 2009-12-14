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
namespace Daemoniq.Core.Commands
{
    public class CommandFactory
    {
        public static ICommand CreateInstance(ServiceAction action)
        {
            LogHelper.EnterFunction(action);
            var command = default(ICommand);
            switch (action)
            {
                case ServiceAction.Console:
                    command = new ConsoleCommand();
                    break;
                case ServiceAction.Install:
                    command = new InstallCommand();
                    break;
                case ServiceAction.Uninstall:
                    command = new UninstallCommand();
                    break;
                case ServiceAction.Run:
                    command = new RunCommand();
                    break;
            }
            LogHelper.LeaveFunction();
            return command;
        }
    }
}