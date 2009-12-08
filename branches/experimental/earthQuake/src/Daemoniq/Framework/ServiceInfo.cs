using System.Collections.Generic;
using Daemoniq.Configuration;

namespace Daemoniq.Framework
{
    public class ServiceInfo
    {
        private readonly List<string> servicesDependedOn;

        public ServiceInfo()
        {
            StartMode = StartMode.Manual;
            servicesDependedOn = new List<string>();
            RecoveryOptions = new ServiceRecoveryOptions();
        }

        public string ServiceName { get; set; }                
        public string DisplayName { get; set; }        
        public string Description { get; set; }
        public StartMode StartMode { get; set; }
        public ServiceRecoveryOptions RecoveryOptions { get; set; }
        public List<string> ServicesDependedOn
        {
            get { return servicesDependedOn; }
        }

        public static ServiceInfo FromConfiguration(ServiceElement serviceElement)
        {
            var serviceInfo = new ServiceInfo();
            serviceInfo.ServiceName = serviceElement.ServiceName;
            serviceInfo.DisplayName = serviceElement.DisplayName;
            serviceInfo.Description = serviceElement.Description;
            serviceInfo.StartMode = serviceElement.StartMode;
            serviceInfo.RecoveryOptions = ServiceRecoveryOptions.FromConfiguration(serviceElement.RecoveryOptions);
            return serviceInfo;
        }
    }
}