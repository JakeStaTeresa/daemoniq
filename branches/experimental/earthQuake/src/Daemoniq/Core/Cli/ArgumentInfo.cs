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
using System.Text;

namespace Daemoniq.Core.Cli
{
    public class ArgumentInfo
    {
        private readonly ArgumentFormat argumentFormat;
        
        public string ShortArgument { get; set; }
        public string LongArgument { get; private set; }
        public string Description { get; set; } 
        public bool Required { get; set; }
        public ArgumentType Type { get; set; }
        public string DefaultValue { get; set; }
        public string[] AcceptedValues { get; set; }               

        public ArgumentInfo(
            string longArgument,
            ArgumentFormat argumentFormat)
        {
            LogHelper.EnterFunction(longArgument, argumentFormat);
            ThrowHelper.ThrowArgumentNullIfNull(longArgument, "longArgument");
            ThrowHelper.ThrowArgumentOutOfRangeIfEmpty(longArgument, "longArgument");
            ThrowHelper.ThrowArgumentNullIfNull(argumentFormat, "argumentFormat");

            LongArgument = longArgument;
            this.argumentFormat = argumentFormat;

            LogHelper.LeaveFunction();
        }

        public ArgumentFormat Format
        {
            get { return argumentFormat; }
        } 

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            if (!Required)
            {
                stringBuilder.Append("[");
            }
            stringBuilder.AppendFormat("{0}{1}",
                Format.LongArgumentPrefix,
                LongArgument);
            if (!string.IsNullOrEmpty(ShortArgument))
            {
                stringBuilder.AppendFormat("|{0}{1}",
                    Format.ShortArgumentPrefix,
                    ShortArgument);
            }

            if (Type == ArgumentType.Normal)
            {
                stringBuilder.AppendFormat("{0}{1}",
                    Format.KeyValueSeparator, "value");
            }

            if (Type == ArgumentType.Password)
            {
                stringBuilder.AppendFormat("[{0}{1}]",
                    Format.KeyValueSeparator, "value");
            }

            if (!Required)
            {
                stringBuilder.Append("]");
            }
            return stringBuilder.ToString();
        }
    }
}