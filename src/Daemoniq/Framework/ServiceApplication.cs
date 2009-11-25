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
using Daemoniq.Core;
using Daemoniq.Core.Cli;
using Daemoniq.Core.Commands;

namespace Daemoniq.Framework
{
    public class ServiceApplication<TService>
        where TService:IServiceInstance, new()
    {
        private readonly IConfigurer configurer;

        public ServiceApplication()
            :this(new DefaultConfigurer())
        {            
        }

        public ServiceApplication(IConfigurer configurer)
        {
            this.configurer = configurer;
        }

        public IConfigurer Configurer { get { return configurer; } }        

        public void Run(string[] arguments)
        {
            LogHelper.EnterFunction(arguments);
            ThrowHelper.ThrowArgumentNullIfNull(arguments, "arguments");

            IConfiguration configuration = new Configuration();
            Parser parser = initializeParser(configuration);
            bool runningAsService = isRunningAsService();
            if (runningAsService)
            {
                string runArgument = string.Format("{0}action{1}run",
                                                   parser.Configuration.ArgumentPrefix,
                                                   parser.Configuration.KeyValueSeparator);
                arguments = new[] { runArgument };
            }

            var parseResult = parser.Parse(arguments);
            if (parseResult.ShowHelp)
            {
                parser.ShowHelp();
                return;
            }
            
            if(parseResult.HasErrors)
            {
                parser.ShowErrors(parseResult);
                return;
            }       

            IServiceInstance serviceInstance = new TService();
            configuration.ServiceName = serviceInstance.ServiceName;
            configuration.DisplayName = serviceInstance.DisplayName;
            configuration.Description = serviceInstance.Description;
            configuration.ServicesDependedOn.AddRange(serviceInstance.ServicesDependedOn);
            configuration.RecoveryOptions = serviceInstance.RecoveryOptions;

            Configurer.Configure(configuration);
            var command = CommandFactory.CreateInstance(configuration.Action);
            command.Execute(configuration, serviceInstance);
            LogHelper.LeaveFunction();
        }

        private Parser initializeParser(IConfiguration configuration)
        {           
            var parser = new Parser();
            parser.Arguments.Add(
                new ArgumentInfo
                    {
                        LongArgument = "action",
                        ShortArgument = "a",
                        Description = "The action you wish to perform. ",
                        Required = true,
                        AcceptedValues = new []{"install", "uninstall", "run", "debug"}
                    });
            parser.Arguments.Add(
                new ArgumentInfo
                    {
                        LongArgument = "credentials",
                        ShortArgument = "c",
                        Description = "The credentials to use to perform the action. ",                        
                        AcceptedValues = new []{"localService", "localSystem", "networkService", "user"}
                    });
            parser.Arguments.Add(
                new ArgumentInfo
                    {
                        LongArgument = "username",
                        ShortArgument = "u",
                        Description = "The username of the credentials to use to perform the action. ",                        
                    });
            parser.Arguments.Add(
                new ArgumentInfo
                    {
                        LongArgument = "password",
                        ShortArgument = "p",
                        Description = "The password of the credentials to use to perform the action. ",                        
                    });
            parser.Arguments.Add(
                new ArgumentInfo
                {
                    LongArgument = "logToConsole",
                    ShortArgument = "l",
                    Description = "Log the output of the install/uninstall operation to the console. ",
                    IsFlag = true,
                    AcceptedValues = new []{"true", "false"},
                    DefaultValue = "true"
                });
            parser.Arguments.Add(
                new ArgumentInfo
                {
                    LongArgument = "showCallStack",
                    ShortArgument = "s",
                    Description = "if an exception occurs at any point during installation, the call stack will be printed to the log. ",
                    IsFlag = true,
                    AcceptedValues = new[] { "true", "false" },
                    DefaultValue = "true"
                });
            parser.Arguments.Add(
                new ArgumentInfo
                {
                    LongArgument = "logFile",
                    ShortArgument = "f",
                    Description = "File to write progress to. If empty, do not write log. Default is (assemblyName).InstallLog",                    
                });

            parser.Contexts.Add(
                new ContextInfo
                {
                        Selector = parseResult => parseResult.Arguments.ContainsKey("action") &&
                                                  parseResult.Arguments["action"] == "install",
                        ValidArguments = new[] { "action", "credentials", "username", "password", "logToConsole", "showCallStack", "logFile" },
                        Action = parseResult => installAction(parseResult, configuration)
                });
            parser.Contexts.Add(
                new ContextInfo
                {
                    Selector = parseResult => parseResult.Arguments.ContainsKey("action") &&
                                              parseResult.Arguments["action"] == "uninstall",
                    ValidArguments = new[] { "action", "logToConsole", "showCallStack", "logFile" },
                    Action = parseResult => uninstallAction(parseResult, configuration)
                });
            parser.Contexts.Add(
                new ContextInfo
                {
                    Selector = parseResult => parseResult.Arguments.ContainsKey("action") &&
                                              parseResult.Arguments["action"] == "debug",
                    ValidArguments = new[] { "action" },
                    Action = parseResult => configuration.Action = ConfigurationAction.Console
                });
            parser.Contexts.Add(
                new ContextInfo
                {
                    Selector = parseResult => parseResult.Arguments.ContainsKey("action") &&
                                              parseResult.Arguments["action"] == "run",
                    ValidArguments = new[] { "action" },
                    Action = parseResult => configuration.Action = ConfigurationAction.Run
                });
            
            return parser;
        }

        private void commonInstallerAction(ParseResult parseResult,
            IConfiguration configuration)
        {
            if (parseResult.Arguments.ContainsKey("logToConsole"))
            {
                configuration.LogToConsole = parseResult.Arguments["logToConsole"] == "true";
            }

            if (parseResult.Arguments.ContainsKey("showCallStack"))
            {
                configuration.ShowCallStack = parseResult.Arguments["showCallStack"] == "true";
            }
            
            if(parseResult.Arguments.ContainsKey("showCallStack"))
            {
                configuration.LogFile = parseResult.Arguments["logFile"];
            }
        }

        private void installAction(ParseResult parseResult, 
            IConfiguration configuration)
        {
            configuration.Action = ConfigurationAction.Install;
            bool credentialsExists = parseResult.Arguments.ContainsKey("credentials");
            if (!credentialsExists ||
                (parseResult.Arguments["credentials"] != "user"))
            {
                if (parseResult.Arguments.ContainsKey("username"))
                {
                    parseResult.Errors.Add(
                        string.Format("Argument '{0}' is not valid in this context.", "username"));
                }
                if (parseResult.Arguments.ContainsKey("password"))
                {
                    parseResult.Errors.Add(
                        string.Format("Argument '{0}' is not valid in this context.", "password"));
                }
            }

            if (credentialsExists)
            {
                string credentials = parseResult.Arguments["credentials"];
                switch (credentials)
                {
                    case "localSystem":
                        configuration.AccountInfo = new AccountInfo(AccountType.LocalSystem);
                        break;
                    case "localService":
                        configuration.AccountInfo = new AccountInfo(AccountType.LocalService);
                        break;
                    case "networkService":
                        configuration.AccountInfo = new AccountInfo(AccountType.NetworkService);
                        break;
                    case "user":
                        if (!parseResult.Arguments.ContainsKey("username"))
                        {
                            parseResult.Errors.Add(
                                string.Format("Argument '{0}' is required in this context.", "username"));
                        }
                        if (!parseResult.Arguments.ContainsKey("password"))
                        {
                            parseResult.Errors.Add(
                                string.Format("Argument '{0}' is required in this context.", "password"));
                        }
                        if (parseResult.Arguments.ContainsKey("username") &&
                            parseResult.Arguments.ContainsKey("password"))
                        {
                            configuration.AccountInfo = new AccountInfo(
                                parseResult.Arguments["username"],
                                parseResult.Arguments["password"]);
                        }
                        break;
                }
            }
            commonInstallerAction(parseResult, configuration);
        }

        private void uninstallAction(ParseResult parseResult,
            IConfiguration configuration)
        {
            configuration.Action = ConfigurationAction.Uninstall;
            commonInstallerAction(parseResult, configuration);
        }

        private  bool isRunningAsService()
        {
            bool returnValue = false;
            try
            {                
                LogHelper.WriteLine("Console [ Title:{0} ]", Console.Title);
            }
            catch
            {
                returnValue = true;
            }
            return returnValue;
        }        
    }    
}
