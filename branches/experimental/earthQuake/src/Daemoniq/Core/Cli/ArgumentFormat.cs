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
    public class ArgumentFormat
    {
        public string LongArgumentPrefix { get; private set; }
        public string ShortArgumentPrefix { get; private set; }
        public string KeyValueSeparator { get; private set; }

        public ArgumentFormat(string argumentPrefix,
            string keyValueSeparator)            
        {
            LogHelper.EnterFunction(argumentPrefix, keyValueSeparator);
            ThrowHelper.ThrowArgumentNullIfNull(argumentPrefix, "argumentPrefix");
            ThrowHelper.ThrowArgumentNullIfNull(keyValueSeparator, "keyValueSeparator");
            ThrowHelper.ThrowArgumentOutOfRangeIfEmpty(keyValueSeparator, "keyValueSeparator");

            LongArgumentPrefix = argumentPrefix;
            ShortArgumentPrefix = argumentPrefix;
            KeyValueSeparator = keyValueSeparator;

            LogHelper.LeaveFunction();
        }

        public ArgumentFormat(
            string longArgumentPrefix,
            string shortArgumentPrefix,
            string keyValueSeparator)
        {
            LogHelper.EnterFunction(longArgumentPrefix, shortArgumentPrefix, keyValueSeparator);
            ThrowHelper.ThrowArgumentNullIfNull(longArgumentPrefix, "longArgumentPrefix");
            ThrowHelper.ThrowArgumentNullIfNull(shortArgumentPrefix, "shortArgumentPrefix");
            ThrowHelper.ThrowArgumentNullIfNull(keyValueSeparator, "keyValueSeparator");
            ThrowHelper.ThrowArgumentOutOfRangeIfEmpty(keyValueSeparator, "keyValueSeparator");

            LongArgumentPrefix = longArgumentPrefix;
            ShortArgumentPrefix = shortArgumentPrefix;
            KeyValueSeparator = keyValueSeparator;

            LogHelper.LeaveFunction();
        }

        private static readonly ArgumentFormat defaultArgumentFormat = 
            new ArgumentFormat("/", "=");

        public static ArgumentFormat Default
        {
            get { return defaultArgumentFormat; }
        }
    }
}