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
    class ArgumentInfo
    {
        private readonly Configuration configuration;
        
        public string ShortArgument { get; set; }
        public string LongArgument { get; set; }
        public string Description { get; set; } 
        public bool Required { get; set; }
        public bool IsFlag { get; set; }
        public string DefaultValue { get; set; }
        public string[] AcceptedValues { get; set; }
        
        public ArgumentInfo()
            :this(Configuration.Default)
        {            
        }

        public ArgumentInfo(Configuration configuration)
        {
            this.configuration = configuration; 
        }

        public Configuration Configuration
        {
            get { return configuration; }
        } 

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            if (!Required)
            {
                stringBuilder.Append("[");
            }
            stringBuilder.AppendFormat("{0}{1}",
                Configuration.ArgumentPrefix,
                LongArgument);
            if (!string.IsNullOrEmpty(ShortArgument))
            {
                stringBuilder.AppendFormat("|{0}{1}",
                    Configuration.ArgumentPrefix,
                    ShortArgument);
            }

            if (!IsFlag)
            {
                stringBuilder.AppendFormat("{0}{1}",
                    Configuration.KeyValueSeparator, "value");
            }

            if (!Required)
            {
                stringBuilder.Append("]");
            }
            return stringBuilder.ToString();
        }
    }
}
