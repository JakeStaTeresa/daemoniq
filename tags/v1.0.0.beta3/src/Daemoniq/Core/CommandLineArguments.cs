using Daemoniq.Framework;

namespace Daemoniq.Core
{
    class CommandLineArguments
    {
        public CommandLineArguments()
        {
            AccountInfo = new AccountInfo(AccountType.User);
        }

        public ConfigurationAction Action { get; set; }
        public AccountInfo AccountInfo { get; set; }
        public bool AllowServiceToInteractWithDesktop { get; set; }
        public string LogFile { get; set; }
        public bool? LogToConsole { get; set; }
        public bool? ShowCallStack { get; set; }
    }
}