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

using Common.Logging;
using Daemoniq.Core;
using Daemoniq.Core.Cli;
using Daemoniq.Core.Commands;

namespace Daemoniq.Framework
{
    public class ServiceApplication
    {
        private static ILog log = LogManager.GetCurrentClassLogger();

        public IConfigurer Configurer { get; set; }

        public ServiceApplication()
        {
            Configurer = new DefaultConfigurer();
        }

        public void Run(string[] arguments)
        {
            ThrowHelper.ThrowArgumentNullIfNull(arguments, "arguments");

            log.Debug(m => m("Running service application..."));            

            var commandLineArguments = new CommandLineArguments();
            Parser parser = initializeParser(commandLineArguments);
            bool runningAsService = isRunningAsService();
            bool interactive = false;
            if (runningAsService)
            {
                string runArgument = string.Format("{0}action{1}run",
                                                   parser.Configuration.ArgumentPrefix,
                                                   parser.Configuration.KeyValueSeparator);
                arguments = new[] { runArgument };
            }
            else
            {
                interactive = arguments.Length == 0;
            }

            Console.WriteLine(parser.HeaderText);
            if(interactive)
            {
                parser.ShowHelp();
            }

            ParseResult parseResult;
            do
            {
                if (interactive)
                {
                    arguments = promptForArguments();
                }

                if(arguments.Length == 1 &&
                       arguments[0].ToLower() == "exit")
                {
                    return;
                }

                parseResult = parser.Parse(arguments);
                if (parseResult.ShowHelp)
                {
                    parser.ShowHelp();
                    if (!interactive)
                    {
                        return;
                    }
                }

                if (parseResult.HasErrors)
                {
                    parser.ShowErrors(parseResult);
                    if (!interactive)
                    {
                        return;
                    }
                }  
            } while (interactive &&
                (parseResult.ShowHelp || 
                parseResult.HasErrors));

            IConfiguration configuration =  Configurer.Configure();
            var command = CommandFactory.CreateInstance(commandLineArguments.Action);
            command.Execute(configuration, commandLineArguments);

            if(interactive)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
            }
            log.Debug(m => m("Done running service application."));            
        }

        private string[] promptForArguments()
        {
            Console.Write(">> ");
            var input = Console.ReadLine();
            var returnValue = new string[]{};
            if(!string.IsNullOrEmpty(input))
            {
                returnValue = input.Split(new[] {" "},
                                          StringSplitOptions.RemoveEmptyEntries);
            }
            return returnValue;
        }

        private Parser initializeParser(
            CommandLineArguments commandLineArguments)
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
                        Type = ArgumentType.Password
                    });
            parser.Arguments.Add(
                new ArgumentInfo
                {
                    LongArgument = "interactive",
                    ShortArgument = "i",
                    Description = "Enables the 'Allow the service to interact with desktop' option in the services.msc applet. This is only valid when credentials is set to 'localSystem'",
                    Type = ArgumentType.Flag,
                    AcceptedValues = new[] { "true", "false" },
                    DefaultValue = "true"
                });
            parser.Arguments.Add(
                new ArgumentInfo
                {
                    LongArgument = "logToConsole",
                    ShortArgument = "l",
                    Description = "Log the output of the install/uninstall operation to the console. ",
                    Type = ArgumentType.Flag,
                    AcceptedValues = new []{"true", "false"},
                    DefaultValue = "true"
                });
            parser.Arguments.Add(
                new ArgumentInfo
                {
                    LongArgument = "showCallStack",
                    ShortArgument = "s",
                    Description = "if an exception occurs at any point during installation, the call stack will be printed to the log. ",
                    Type = ArgumentType.Flag,
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
                        ValidArguments = new[] { "action", "credentials", "username", "password", "interactive", "logToConsole", "showCallStack", "logFile" },
                        Action = parseResult => installAction(parseResult, commandLineArguments)
                });
            parser.Contexts.Add(
                new ContextInfo
                {
                    Selector = parseResult => parseResult.Arguments.ContainsKey("action") &&
                                              parseResult.Arguments["action"] == "uninstall",
                    ValidArguments = new[] { "action", "logToConsole", "showCallStack", "logFile" },
                    Action = parseResult => uninstallAction(parseResult, commandLineArguments)
                });
            parser.Contexts.Add(
                new ContextInfo
                {
                    Selector = parseResult => parseResult.Arguments.ContainsKey("action") &&
                                              parseResult.Arguments["action"] == "debug",
                    ValidArguments = new[] { "action" },
                    Action = parseResult => commandLineArguments.Action = ConfigurationAction.Console
                });
            parser.Contexts.Add(
                new ContextInfo
                {
                    Selector = parseResult => parseResult.Arguments.ContainsKey("action") &&
                                              parseResult.Arguments["action"] == "run",
                    ValidArguments = new[] { "action" },
                    Action = parseResult => commandLineArguments.Action = ConfigurationAction.Run
                });
            
            return parser;
        }

        private void commonInstallerAction(ParseResult parseResult,
            CommandLineArguments commandLineArguments)
        {
            if (parseResult.Arguments.ContainsKey("logToConsole"))
            {
                commandLineArguments.LogToConsole = parseResult.Arguments["logToConsole"] == "true";
            }

            if (parseResult.Arguments.ContainsKey("showCallStack"))
            {
                commandLineArguments.ShowCallStack = parseResult.Arguments["showCallStack"] == "true";
            }
            
            if(parseResult.Arguments.ContainsKey("showCallStack"))
            {
                commandLineArguments.LogFile = parseResult.Arguments["logFile"];
            }
        }

        private void installAction(ParseResult parseResult,
            CommandLineArguments commandLineArguments)
        {
            commandLineArguments.Action = ConfigurationAction.Install;
            bool credentialsExists = parseResult.Arguments.ContainsKey("credentials");
            if (!credentialsExists)
            {
                // all other arguments are invalid when 
                // credentials does not exist
                validateInstallArguments(parseResult,
                    new [] { "action" });
            }

            if (credentialsExists)
            {
                string credentials = parseResult.Arguments["credentials"];
                switch (credentials)
                {
                    case "localSystem":
                        validateInstallArguments(parseResult,
                            new [] { "action","credentials", "interactive" });
                        commandLineArguments.AccountInfo = new AccountInfo(AccountType.LocalSystem);                        
                        break;
                    case "localService":
                        validateInstallArguments(parseResult,
                            new[] { "action", "credentials" });
                        commandLineArguments.AccountInfo = new AccountInfo(AccountType.LocalService);
                        break;
                    case "networkService":
                        validateInstallArguments(parseResult,
                            new[] { "action", "credentials" });
                        commandLineArguments.AccountInfo = new AccountInfo(AccountType.NetworkService);
                        break;
                    case "user":
                        validateInstallArguments(parseResult,
                            new[] { "action", "credentials", "username", "password" });
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
                            commandLineArguments.AccountInfo = new AccountInfo(
                                parseResult.Arguments["username"],
                                parseResult.Arguments["password"]);
                        }
                        break;
                }
            }
            commonInstallerAction(parseResult, commandLineArguments);
        }

        private void validateInstallArguments(ParseResult parseResult,
            string[] validArguments)
        {
            foreach (var argumentName in parseResult.Arguments.Keys)
            {
                string argument = argumentName;
                if (Array.FindAll(validArguments, s => s == argument).Length == 0)
                {
                    parseResult.Errors.Add(string.Format("Argument '{0}' is not valid in this context.",
                                                  argument));
                    continue;
                }
            }
        }

        private void uninstallAction(ParseResult parseResult,
            CommandLineArguments commandLineArguments)
        {
            commandLineArguments.Action = ConfigurationAction.Uninstall;
            commonInstallerAction(parseResult, commandLineArguments);
        }

        private  bool isRunningAsService()
        {
            bool returnValue = false;
            try
            {                
                log.DebugFormat("Console [ Title:{0} ]", Console.Title);
            }
            catch
            {
                returnValue = true;
            }
            return returnValue;
        }        
    }    
}