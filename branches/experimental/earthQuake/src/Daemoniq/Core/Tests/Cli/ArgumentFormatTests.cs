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
using Daemoniq.Core.Cli;
using NUnit.Framework;

namespace Daemoniq.Core.Tests.Cli
{
    [TestFixture]
    public class ArgumentFormatTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WillThrowArgumentNullIfLongArgumentIsNull()
        {
            new ArgumentFormat(null, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WillThrowArgumentNullIfShortArgumentIsNull()
        {
            new ArgumentFormat("/", null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WillThrowArgumentNullIfKeyValueSeparatorIsNull()
        {
            new ArgumentFormat("/", null);
        }

        [Test]
        public void AllowEmptyLongArgumentTest()
        {
            new ArgumentFormat("", "-", ":");
        }

        [Test]
        public void AllowEmptyShortArgumentTest()
        {
            new ArgumentFormat("/", "", ":");
        }

        [Test]
        public void AllowEmptyArgumentTest()
        {
            new ArgumentFormat("", ":");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WillThrowArgumentOutOfRangeIfKeyValueSeparatorIsBlank()
        {
            new ArgumentFormat("/", "");
        }

        [Test]
        public void DefaultArgumentFormatTest()
        {
            var argumentFormat = ArgumentFormat.Default;
            Assert.AreEqual("/", argumentFormat.LongArgumentPrefix);
            Assert.AreEqual("/", argumentFormat.ShortArgumentPrefix);
            Assert.AreEqual("=", argumentFormat.KeyValueSeparator);
        }

        public void CustomArgumentFormatTest()
        {
            var argumentFormat = new ArgumentFormat("--", "-", ":");
            Assert.AreEqual("--", argumentFormat.LongArgumentPrefix);
            Assert.AreEqual("-", argumentFormat.ShortArgumentPrefix);
            Assert.AreEqual(":", argumentFormat.KeyValueSeparator);
        }
    }
}