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
namespace Daemoniq.Core.Cli
{
    class Configuration
    {
        private static Configuration defaultConfiguration = 
            new Configuration{
                ArgumentPrefix = "/",
                KeyValueSeparator = "=",
                ConsoleWidth = 78};

        public string ArgumentPrefix { get; set; }
        public string KeyValueSeparator { get; set; }
        public int ConsoleWidth{ get; set; }

        public static Configuration Default
        {
            get { return defaultConfiguration; }
        }
    }
}
