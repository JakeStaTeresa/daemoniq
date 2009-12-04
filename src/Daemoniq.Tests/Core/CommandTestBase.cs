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
using Daemoniq.Core.Commands;
using Daemoniq.Framework;
using NUnit.Framework;

namespace Daemoniq.Tests.Core
{
    public abstract class CommandTestBase
    {
        protected ConfigurationAction Action { get; set; }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WillThrowArgumentNullIfConfigurationIsNull()
        {
            createCommand().Execute(null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WillThrowArgumentNullIfCommandLineArgumentsIsNull()
        {
            createCommand().Execute(new Daemoniq.Framework.Configuration(), null);
        }

        private ICommand createCommand()
        {
            return CommandFactory.CreateInstance(Action);
        }

    }
}
