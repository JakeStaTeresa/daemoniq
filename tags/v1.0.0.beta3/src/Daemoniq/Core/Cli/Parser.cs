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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Daemoniq.Core.Cli
{
    class Parser
    {
        private readonly Configuration configuration;
        private readonly string headerText;
        private readonly string programName;
        private string usageText;
        private string argumentList;
        private readonly List<ArgumentInfo> arguments;
        private readonly List<ContextInfo> contexts;

        public Parser()
            :this(Configuration.Default)
        {            
        }

        public Parser(Configuration configuration)
        {
            this.configuration = configuration; 
            arguments = new List<ArgumentInfo>();
            contexts = new List<ContextInfo>();

            ProcessModule module = Process.GetCurrentProcess().MainModule;
            if(module == null)
            {
                return;
            }
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(module.FileName);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(
                string.Format("{0} {1}",
                    versionInfo.OriginalFilename, versionInfo.ProductVersion));
            stringBuilder.AppendLine("Copyright (C) 2009");
            stringBuilder.AppendLine(versionInfo.CompanyName);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Powered By: Daemoniq");
            stringBuilder.AppendLine("            Windows service hosting for mere mortals");
            headerText = stringBuilder.ToString();
            programName = Path.GetFileNameWithoutExtension(versionInfo.FileName);

            Arguments.Add(
                new ArgumentInfo
                {
                    LongArgument = "help",
                    ShortArgument = "h",
                    Type =  ArgumentType.Flag,
                    AcceptedValues = new []{ "true", "false"},
                    DefaultValue = "true",
                    Description = "Show this help text.",
                });
        }        
        
        public string HeaderText
        {
            get{ return headerText; }
        }

        public string UsageText
        {
            get
            {
                if(string.IsNullOrEmpty(usageText))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("Usage: ");
                    stringBuilder.AppendFormat("   {0}", programName);
                    if(arguments.Count > 0)
                    {
                        stringBuilder.Append(" ");
                        foreach (var argumentInfo in Arguments)
                        {
                            stringBuilder.AppendFormat("{0} ", argumentInfo);
                        }
                    }
                    usageText = stringBuilder.ToString();
                }
                return usageText;
            }
        }

        public string ArgumentList
        {
            get
            {
                if (string.IsNullOrEmpty(argumentList))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    if (arguments.Count > 0)
                    {
                        stringBuilder.AppendLine("Available arguments are:");
                        int maxLongArgumentLength = 0;
                        foreach (var argumentInfo in Arguments)
                        {
                            if(argumentInfo.LongArgument.Length > maxLongArgumentLength)
                            {
                                maxLongArgumentLength = argumentInfo.LongArgument.Length;
                            }
                        }
                        
                        foreach (var argumentInfo in Arguments)
                        {
                            if (argumentInfo.AcceptedValues != null && argumentInfo.AcceptedValues.Length > 0)
                            {
                                argumentInfo.Description = string.Format("{0} Accepted values are '{1}'.",
                                                                         argumentInfo.Description,
                                                                         string.Join("', '", argumentInfo.AcceptedValues));                                
                            }
                            writeFormatted(stringBuilder,
                                    maxLongArgumentLength,
                                    argumentInfo.LongArgument,
                                    argumentInfo.Description);                            
                        }
                    }
                    argumentList = stringBuilder.ToString();
                }
                return argumentList;
            }
        }

        public Configuration Configuration
        {
            get { return configuration; }
        }

        public List<ArgumentInfo> Arguments
        {
            get { return arguments; }
        }

        public List<ContextInfo> Contexts
        {
            get { return contexts; }
        }

        public ParseResult Parse(string[] args)
        {
            
            Dictionary<string, ArgumentInfo> argumentMap = 
                createArgumentMap();

            ParseResult parseResult = parseArguments(args, argumentMap);

            checkRequiredArguments(parseResult);

            checkArgumentValues(parseResult, argumentMap);            

            performContextActions(parseResult);

            return parseResult;
        }      

        public void ShowHelp()
        {
            Console.WriteLine(UsageText);
            Console.WriteLine();     
            Console.WriteLine(ArgumentList);
        }

        public  void ShowErrors(ParseResult parseResult)
        {
            Console.WriteLine(UsageText);
            Console.WriteLine();
            Console.WriteLine("The following errors occured in this operation:");
            int counter = 1;
            var stringBuilder = new StringBuilder();
            foreach (var error in parseResult.Errors)
            {
                writeFormatted(stringBuilder, 3, counter.ToString(), error);                
                counter++;
            }
            Console.WriteLine(stringBuilder);
            Console.WriteLine();
            Console.WriteLine(ArgumentList);
        }
      
        private Dictionary<string, ArgumentInfo> createArgumentMap()
        {
            var argumentMap = 
                new Dictionary<string, ArgumentInfo>();
            foreach (var argumentInfo in arguments)
            {
                if(argumentMap.ContainsKey(argumentInfo.ShortArgument))
                {
                    throw new InvalidOperationException();
                }

                if (argumentMap.ContainsKey(argumentInfo.LongArgument))
                {
                    throw new InvalidOperationException();
                }

                argumentMap.Add(argumentInfo.ShortArgument, argumentInfo);
                argumentMap.Add(argumentInfo.LongArgument, argumentInfo);
            }
            return argumentMap;
        }

        private List<ArgumentInfo> getRequiredArguments()
        {
            var requiredArgumentInfos = new List<ArgumentInfo>();           
            foreach (var argumentInfo in arguments)
            {                
                if (argumentInfo.Required)
                {
                    requiredArgumentInfos.Add(argumentInfo);
                }

            }
            return requiredArgumentInfos;
        }

        private ParseResult parseArguments(string[] argumentArray,
            IDictionary<string, ArgumentInfo> argumentMap)
        {
            var parseResult = new ParseResult();
            for (int i = 0; i < argumentArray.Length; i++)
            {
                string argument = argumentArray[i].Trim();
                if (!argument.StartsWith(configuration.ArgumentPrefix))
                {
                    parseResult.Errors.Add(string.Format("Argument '{0}' must start with prefix '{1}'",
                                                  argument, configuration.ArgumentPrefix));
                    continue;
                }

                argument = argument.Substring(1);
                ArgumentInfo argumentInfo;
                if (!argument.Contains(configuration.KeyValueSeparator))
                {
                    if (!argumentMap.ContainsKey(argument))
                    {
                        parseResult.Errors.Add(string.Format("Argument '{0}' must have the following key value separator '{1}'",
                                                      argument, configuration.KeyValueSeparator));
                        continue;
                    }

                    argumentInfo = argumentMap[argument];                    
                    if (argumentInfo.Type == ArgumentType.Normal)
                    {
                        parseResult.Errors.Add(string.Format("Argument '{0}' must have the following key value separator '{1}'",
                                                      argument, configuration.KeyValueSeparator));
                        continue;
                    }

                    if(argumentInfo.Type == ArgumentType.Flag)
                    {
                        if (string.IsNullOrEmpty(argumentInfo.DefaultValue))
                        {
                            parseResult.Errors.Add(string.Format("Flag argument '{0}' must have a default value.",
                                                          argument));
                            continue;
                        }

                        argument = string.Format("{0}{1}{2}",
                                                 argument,
                                                 configuration.KeyValueSeparator,
                                                 argumentInfo.DefaultValue);    
                    }

                    if (argumentInfo.Type == ArgumentType.Password)
                    {
                        string password = getPassword();
                        argument = string.Format("{0}{1}{2}",
                                                 argument,
                                                 configuration.KeyValueSeparator,
                                                 password);
                    }
                }

                string[] kv = argument.Split(new[] { configuration.KeyValueSeparator },
                                             StringSplitOptions.RemoveEmptyEntries);

                if(kv.Length == 1 &&
                    argument.EndsWith(configuration.KeyValueSeparator))
                {
                    parseResult.Errors.Add(string.Format("Argument '{0}' does not have a value.",
                                                  kv[0]));
                    continue;                                       
                }

                string argumentName = kv[0];
                string argumentValue = kv[1];

                if (!argumentMap.ContainsKey(argumentName))
                {
                    parseResult.Errors.Add(string.Format("Argument '{0}' is not valid in this context.",
                                                  argumentName));
                    continue;
                }

                argumentInfo = argumentMap[argumentName];
                parseResult.Arguments.Add(argumentInfo.LongArgument, argumentValue);                                
            }

            return parseResult;
        }

        private string getPassword()
        {
            Console.WriteLine("Please enter you password in the space provided:");
            Console.Write("  Password: ");                        
            string password = "";
            ConsoleKeyInfo info;
            do
            {
                info = Console.ReadKey(true);
                if (info.Key == ConsoleKey.Enter)
                {
                    if(string.IsNullOrEmpty(password))
                    {
                        Console.Beep();
                        continue;
                    }

                    break;
                }
                            
                if (info.Key != ConsoleKey.Backspace)
                {
                    char passwordChar = info.KeyChar; 
                    if(char.IsLetter(passwordChar) ||
                       char.IsNumber(passwordChar) ||
                       char.IsSymbol(passwordChar))
                    {
                        password += info.KeyChar;
                        Console.Write("*");    
                    }
                    else
                    {
                        Console.Beep();
                    }
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.CursorLeft -= 1;
                        Console.Write(" ");
                        Console.CursorLeft -= 1;
                    }
                    else
                    {
                        Console.Beep();
                    }
                }
            } while (true);
            Console.WriteLine();
            return password;
        }

        private void checkRequiredArguments(ParseResult parseResult)
        {
            var parseErrors = new List<string>(parseResult.Errors);
            parseResult.Errors.Clear();
            var requiredArgumentInfos = getRequiredArguments();
            foreach (var argumentInfo in requiredArgumentInfos)
            {
                if (!parseResult.Arguments.ContainsKey(argumentInfo.ShortArgument) &&
                   !parseResult.Arguments.ContainsKey(argumentInfo.LongArgument))
                {
                    parseResult.Errors.Add(string.Format("Required argument '{0}' is not in the parameter list.",
                                                         argumentInfo.LongArgument));
                    continue;
                }
            }
            parseResult.Errors.AddRange(parseErrors);
        }

        private void checkArgumentValues(ParseResult parseResult,
            IDictionary<string, ArgumentInfo> argumentMap)
        {
            foreach (var argument in parseResult.Arguments)
            {
                var argumentName = argument.Key;
                var argumentValue = argument.Value;                

                var argumentInfo = argumentMap[argumentName];
                if (argumentInfo.AcceptedValues != null &&
                    argumentInfo.AcceptedValues.Length > 0)
                {
                    if (Array.FindAll(argumentInfo.AcceptedValues, s => s == argumentValue).Length == 0)
                    {
                        parseResult.Errors.Add(string.Format("Value '{0}' for argument '{1}' is not in the list of accepted values. Accepted values are '{2}'.",
                                                      argumentValue, argumentName, string.Join("', '", argumentInfo.AcceptedValues))); 
                        continue;
                    }
                }
            }
        }

        private void performContextActions(ParseResult parseResult)
        {
            ContextInfo selectedContext = null;
            foreach (ContextInfo context in Contexts)
            {
                if(context.Selector(parseResult))
                {
                    selectedContext = context;
                    break;
                }
            }

            if(selectedContext != null)
            {
                foreach (var argumentName in parseResult.Arguments.Keys)
                {
                    string argument = argumentName;
                    if (Array.FindAll(selectedContext.ValidArguments, s => s == argument).Length == 0)
                    {
                        parseResult.Errors.Add(string.Format("Argument '{0}' is not valid in this context.",
                                                      argument));
                        continue;
                    }
                }

                selectedContext.Action(parseResult);
            }            
        }

        private void writeFormatted(StringBuilder stringBuilder,
            int firstColumnLength,
            string firstColumnText,
            string secondColumnText)
        {
            stringBuilder.AppendFormat("  {0}{1} : ",
                firstColumnText,
                new string(' ', firstColumnLength - firstColumnText.Length));

            int descriptionLength = configuration.ConsoleWidth - firstColumnLength - 5;
            int chunkCount = secondColumnText.Length / descriptionLength;
            int remainder = secondColumnText.Length % descriptionLength;
            if (remainder != 0)
            {
                chunkCount++;
            }

            int charAt = 0;
            for (int i = 0; i < chunkCount; i++)
            {
                int length = descriptionLength;
                if (i == chunkCount - 1)
                {
                    length = remainder;
                }
                string chunk = secondColumnText.Substring(charAt, length).Trim();
                charAt += descriptionLength;
                if (i == 0)
                {
                    stringBuilder.AppendLine(chunk);
                }
                else
                {
                    stringBuilder.AppendFormat("{0}{1}{2}",
                                               new string(' ', firstColumnLength + 5),
                                               chunk,
                                               Environment.NewLine);
                }
            }
        }
    }
}
