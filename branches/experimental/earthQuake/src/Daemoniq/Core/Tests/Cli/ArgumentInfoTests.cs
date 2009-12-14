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
    public class ArgumentInfoTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WillThrowArgumentNullIfLongArgumentIsNull()
        {
            new ArgumentInfo(null, ArgumentFormat.Default);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WillThrowArgumentOutOfRangeIfLongArgumentIsEmpty()
        {
            new ArgumentInfo(string.Empty, ArgumentFormat.Default);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WillThrowArgumentNullIfArgumentFormatIsNull()
        {
            new ArgumentInfo("sample", null);
        }

        [Test]
        public void ConstuctionTest()
        {
            var argumentInfo = new ArgumentInfo("sample", ArgumentFormat.Default);
            Assert.AreEqual("[/sample=value]", argumentInfo.ToString());
        }

        [Test]
        public void RequiredArgumentTest()
        {
            var argumentInfo = new ArgumentInfo("sample", ArgumentFormat.Default);
            argumentInfo.Required = true;
            Assert.AreEqual("/sample=value", argumentInfo.ToString());
        }

        [Test]
        public void ShortArgumentTest()
        {
            var argumentInfo = new ArgumentInfo("sample", ArgumentFormat.Default);
            argumentInfo.ShortArgument = "s";
            Assert.AreEqual("[/sample|/s=value]", argumentInfo.ToString());
        }

        [Test]
        public void RequiredAndShortArgumentTest()
        {
            var argumentInfo = new ArgumentInfo("sample", ArgumentFormat.Default);
            argumentInfo.Required = true;
            argumentInfo.ShortArgument = "s";
            Assert.AreEqual("/sample|/s=value", argumentInfo.ToString());
        }

        [Test]
        public void FlagArgumentTest()
        {
            var argumentInfo = new ArgumentInfo("sample", ArgumentFormat.Default);
            argumentInfo.Type = ArgumentType.Flag;
            Assert.AreEqual("[/sample]", argumentInfo.ToString());
        }

        [Test]
        public void RequiredAndFlagArgumentTest()
        {
            var argumentInfo = new ArgumentInfo("sample", ArgumentFormat.Default);
            argumentInfo.Type = ArgumentType.Flag;
            argumentInfo.Required = true;
            Assert.AreEqual("/sample", argumentInfo.ToString());
        }

        [Test]
        public void RequiredAndFlagAndShortArgumentTest()
        {
            var argumentInfo = new ArgumentInfo("sample", ArgumentFormat.Default);
            argumentInfo.ShortArgument = "s";
            argumentInfo.Type = ArgumentType.Flag;
            argumentInfo.Required = true;
            Assert.AreEqual("/sample|/s", argumentInfo.ToString());
        }

        [Test]
        public void PasswordArgumentTest()
        {
            var argumentInfo = new ArgumentInfo("sample", ArgumentFormat.Default);
            argumentInfo.Type = ArgumentType.Password;
            Assert.AreEqual("[/sample[=value]]", argumentInfo.ToString());
        }

        [Test]
        public void RequiredPasswordArgumentTest()
        {
            var argumentInfo = new ArgumentInfo("sample", ArgumentFormat.Default);
            argumentInfo.Type = ArgumentType.Password;
            argumentInfo.Required = true;
            Assert.AreEqual("/sample[=value]", argumentInfo.ToString());
        }
        
        [Test]
        public void RequiredPasswordAndShortArgumentTest()
        {
            var argumentInfo = new ArgumentInfo("sample", ArgumentFormat.Default);
            argumentInfo.Type = ArgumentType.Password;
            argumentInfo.Required = true;
            argumentInfo.ShortArgument = "s";
            Assert.AreEqual("/sample|/s[=value]", argumentInfo.ToString());
        }

        [Test]
        public void CustomArgumentFormatTest()
        {
            var argumentInfo = new ArgumentInfo("sample", new ArgumentFormat("--", "-", ":"));
            Assert.AreEqual("[--sample:value]", argumentInfo.ToString());
        }

        [Test]
        public void ShortArgumentWithCustomArgumentFormatTest()
        {
            var argumentInfo = new ArgumentInfo("sample", new ArgumentFormat("--", "-", ":"));
            argumentInfo.ShortArgument = "s";
            Assert.AreEqual("[--sample|-s:value]", argumentInfo.ToString());
        }

        [Test]
        public void RequiredAndShortArgumentWithCustomArgumentFormatTest()
        {
            var argumentInfo = new ArgumentInfo("sample", new ArgumentFormat("--", "-", ":"));
            argumentInfo.ShortArgument = "s";
            argumentInfo.Required = true;
            Assert.AreEqual("--sample|-s:value", argumentInfo.ToString());
        }

        [Test]
        public void FlagArgumentWithCustomArgumentFormatTest()
        {
            var argumentInfo = new ArgumentInfo("sample", new ArgumentFormat("--", "-", ":"));
            argumentInfo.Type = ArgumentType.Flag;
            Assert.AreEqual("[--sample]", argumentInfo.ToString());
        }

        [Test]
        public void RequiredAndFlagArgumentWithCustomArgumentFormatTest()
        {
            var argumentInfo = new ArgumentInfo("sample", new ArgumentFormat("--", "-", ":"));
            argumentInfo.Type = ArgumentType.Flag;
            argumentInfo.Required = true;
            Assert.AreEqual("--sample", argumentInfo.ToString());
        }

        [Test]
        public void RequiredAndFlagAndShortArgumentWithCustomArgumentFormatTest()
        {
            var argumentInfo = new ArgumentInfo("sample", new ArgumentFormat("--", "-", ":"));
            argumentInfo.ShortArgument = "s";
            argumentInfo.Type = ArgumentType.Flag;
            argumentInfo.Required = true;
            Assert.AreEqual("--sample|-s", argumentInfo.ToString());
        }

        [Test]
        public void PasswordArgumentWithCustomArgumentFormatTest()
        {
            var argumentInfo = new ArgumentInfo("sample", new ArgumentFormat("--", "-", ":"));
            argumentInfo.Type = ArgumentType.Password;
            Assert.AreEqual("[--sample[:value]]", argumentInfo.ToString());
        }

        [Test]
        public void RequiredPasswordArgumentWithCustomArgumentFormatTest()
        {
            var argumentInfo = new ArgumentInfo("sample", new ArgumentFormat("--", "-", ":"));
            argumentInfo.Type = ArgumentType.Password;
            argumentInfo.Required = true;
            Assert.AreEqual("--sample[:value]", argumentInfo.ToString());
        }

        [Test]
        public void RequiredPasswordAndShortArgumentWithCustomArgumentFormatTest()
        {
            var argumentInfo = new ArgumentInfo("sample", new ArgumentFormat("--", "-", ":"));
            argumentInfo.Type = ArgumentType.Password;
            argumentInfo.Required = true;
            argumentInfo.ShortArgument = "s";
            Assert.AreEqual("--sample|-s[:value]", argumentInfo.ToString());
        }
    }
}