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
using System.Collections.Generic;

namespace Daemoniq.Framework
{
    public interface IConfiguration
    {
        string ServiceName { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        List<string> ServicesDependedOn { get; }
        StartMode StartMode { get; set; }
        AccountInfo AccountInfo { get; set; }
        ConfigurationAction Action { get; set; }
        ServiceRecoveryOptions RecoveryOptions { get; }
        string LogFile { get; set; }
        bool? LogToConsole { get; set; }
        bool? ShowCallStack { get; set; }
    }
}