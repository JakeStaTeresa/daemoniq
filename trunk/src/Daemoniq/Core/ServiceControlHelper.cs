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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ServiceProcess;

using Common.Logging;
using Daemoniq.Framework;
using Microsoft.Win32;

namespace Daemoniq.Core
{
    static class ServiceControlHelper
    {
        private static ILog log = LogManager.GetCurrentClassLogger();

        #region "SERVICE RECOVERY INTEROP"
        // ReSharper disable InconsistentNaming
        enum SC_ACTION_TYPE
        {
            None = 0,
            RestartService = 1,
            RebootComputer = 2,
            RunCommand = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SC_ACTION
        {
            public SC_ACTION_TYPE Type;
            public uint Delay;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)] 
        struct SERVICE_FAILURE_ACTIONS
        {
            public int dwResetPeriod;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpRebootMsg;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpCommand; 
            public int cActions;
            public IntPtr lpsaActions;
        }

        private const int SERVICE_CONFIG_FAILURE_ACTIONS = 2;

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ChangeServiceConfig2(
            IntPtr hService,
            int dwInfoLevel,
            IntPtr lpInfo);

        [DllImport("advapi32.dll", EntryPoint = "QueryServiceConfig2", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int QueryServiceConfig2(
            IntPtr hService,
            int dwInfoLevel,
            IntPtr lpBuffer,
            uint cbBufSize,
            out uint pcbBytesNeeded);

        // ReSharper restore InconsistentNaming
        #endregion

        #region "GRANT SHUTDOWN INTEROP"
        // ReSharper disable InconsistentNaming
        [StructLayout(LayoutKind.Sequential)]
        struct LUID_AND_ATTRIBUTES
        {
            public long Luid;
            public UInt32 Attributes;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privileges;

        }

        [DllImport("advapi32.dll")]
        private static extern bool
            AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges,
            [MarshalAs(UnmanagedType.Struct)] ref TOKEN_PRIVILEGES NewState, int BufferLength,
           IntPtr PreviousState, ref int ReturnLength);


        [DllImport("advapi32.dll")]
        private static extern bool
            LookupPrivilegeValue(string lpSystemName, string lpName, ref long lpLuid);

        [DllImport("advapi32.dll")]
        private static extern bool
            OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

        private const int TOKEN_ADJUST_PRIVILEGES = 32;
        private const int TOKEN_QUERY = 8;
        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        private const int SE_PRIVILEGE_ENABLED = 2;
        // ReSharper restore InconsistentNaming
        #endregion
       
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public static void SetServiceRecoveryOptions(
            string serviceName,
            ServiceRecoveryOptions recoveryOptions)
        {
            ThrowHelper.ThrowArgumentNullIfNull(serviceName, "serviceName");
            ThrowHelper.ThrowArgumentOutOfRangeIfEmpty(serviceName, "serviceName");

            log.Debug(m => m("Setting service recovery options..."));            

            bool requiresShutdownPriveleges =
                recoveryOptions.FirstFailureAction == ServiceRecoveryAction.RestartTheComputer ||
                recoveryOptions.SecondFailureAction == ServiceRecoveryAction.RestartTheComputer ||
                recoveryOptions.SubsequentFailureActions == ServiceRecoveryAction.RestartTheComputer;
            if(requiresShutdownPriveleges)
            {
                grantShutdownPrivileges();
            }

            int actionCount = 3;
            var restartServiceAfter = (uint)TimeSpan.FromMinutes(
                recoveryOptions.MinutesToRestartService).TotalMilliseconds;

            IntPtr failureActionsPointer = IntPtr.Zero;
            IntPtr actionPointer = IntPtr.Zero;

            ServiceController controller = null;
            try
            {
                // Open the service
                controller = new ServiceController(serviceName);
                
                // Set up the failure actions
                var failureActions = new SERVICE_FAILURE_ACTIONS();
                failureActions.dwResetPeriod = (int)TimeSpan.FromDays(recoveryOptions.DaysToResetFailAcount).TotalSeconds;
                failureActions.cActions = actionCount;
                failureActions.lpRebootMsg = recoveryOptions.RebootMessage;

                // allocate memory for the individual actions
                actionPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SC_ACTION)) * actionCount);
                ServiceRecoveryAction[] actions = { recoveryOptions.FirstFailureAction,
                                                    recoveryOptions.SecondFailureAction,
                                                    recoveryOptions.SubsequentFailureActions };
                for (int i = 0; i < actions.Length; i++)
                {
                    ServiceRecoveryAction action = actions[i];
                    var scAction = getScAction(action, restartServiceAfter);
                    Marshal.StructureToPtr(scAction, (IntPtr)((Int64)actionPointer + (Marshal.SizeOf(typeof(SC_ACTION))) * i), false);
                }
                failureActions.lpsaActions = actionPointer;

                string command = recoveryOptions.CommandToLaunchOnFailure;
                if(command != null)
                {
                    failureActions.lpCommand = command;
                }
                
                failureActionsPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SERVICE_FAILURE_ACTIONS)));
                Marshal.StructureToPtr(failureActions, failureActionsPointer, false);

                // Make the change
                bool success = ChangeServiceConfig2(
                    controller.ServiceHandle.DangerousGetHandle(),
                    SERVICE_CONFIG_FAILURE_ACTIONS,
                    failureActionsPointer);

                // Check that the change occurred
                if (!success)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to change the Service configuration.");
                }
            }
            finally
            {
                if (failureActionsPointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(failureActionsPointer);
                }

                if (actionPointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(actionPointer);
                }

                if (controller != null)
                {
                    controller.Close();
                }

                log.Debug(m => m("Done setting service recovery options."));   
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public static ServiceRecoveryOptions GetServiceRecoveryOptions(
            string serviceName)
        {
            ThrowHelper.ThrowArgumentNullIfNull(serviceName, "serviceName");
            ThrowHelper.ThrowArgumentOutOfRangeIfEmpty(serviceName, "serviceName");

            log.Debug(m => m("Getting service recovery options..."));            

            // 8KB is the largest buffer supported by QueryServiceConfig2
            const int bufferSize = 1024 * 8;

            IntPtr bufferPtr = IntPtr.Zero;

            ServiceRecoveryOptions recoveryOptions;
            ServiceController controller = null;
            try
            {
                // Open the service
                controller = new ServiceController(serviceName);

                uint dwBytesNeeded;

                // Allocate memory for struct
                bufferPtr = Marshal.AllocHGlobal(bufferSize);
                int queryResult = QueryServiceConfig2(
                    controller.ServiceHandle.DangerousGetHandle(),
                    SERVICE_CONFIG_FAILURE_ACTIONS,
                    bufferPtr,
                    bufferSize,
                    out dwBytesNeeded);

                if (queryResult == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to query the Service configuration.");
                }

                // Cast the buffer to a QUERY_SERVICE_CONFIG struct
                SERVICE_FAILURE_ACTIONS config =
                    (SERVICE_FAILURE_ACTIONS) Marshal.PtrToStructure(bufferPtr, typeof (SERVICE_FAILURE_ACTIONS));

                recoveryOptions = new ServiceRecoveryOptions();
                recoveryOptions.DaysToResetFailAcount = (int)
                    TimeSpan.FromSeconds(config.dwResetPeriod).TotalDays;
                recoveryOptions.RebootMessage = config.lpRebootMsg;
                recoveryOptions.CommandToLaunchOnFailure = config.lpCommand;                

                int actionCount = config.cActions;
                if (actionCount != 0)
                {
                    uint millisecondsToRestartService = 0;
                    SC_ACTION[] actions = new SC_ACTION[actionCount];
                    for (int i = 0; i < config.cActions; i++)
                    {
                        SC_ACTION action = (SC_ACTION)Marshal.PtrToStructure(
                            (IntPtr)(config.lpsaActions.ToInt32() + (Marshal.SizeOf(typeof(SC_ACTION)) * i)), 
                            typeof(SC_ACTION));
                        actions[i] = action;
                        millisecondsToRestartService = action.Delay;
                    }

                    recoveryOptions.FirstFailureAction = getServiceRecoveryAction(actions[0]);
                    recoveryOptions.SecondFailureAction = getServiceRecoveryAction(actions[1]);
                    recoveryOptions.SubsequentFailureActions = getServiceRecoveryAction(actions[2]);
                    recoveryOptions.MinutesToRestartService =
                        (int) TimeSpan.FromMilliseconds(millisecondsToRestartService).TotalMinutes;
                }
            }
            finally
            {
                // Clean up
                if (bufferPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(bufferPtr);
                }

                if (controller != null)
                {
                    controller.Close();
                }

                log.Debug(m => m("Done getting service recovery options."));            
            }

            
            return recoveryOptions;
        }

        public static bool IsServiceInstalled(string serviceName)
        {
            ThrowHelper.ThrowArgumentNullIfNull(serviceName, "serviceName");
            ThrowHelper.ThrowArgumentOutOfRangeIfEmpty(serviceName, "serviceName");

            log.Debug(m => m("Checking if service '{0}' is already installed...", serviceName));            

            bool returnValue = GetInstalledServices().Contains(serviceName);
            log.Debug(m => m("Service '{0}' is {1} installed.", serviceName, returnValue ? "already" : "not yet"));
            return returnValue;
        }        

        public static void AllowServiceToInteractWithDesktop(string serviceName)
        {
            ThrowHelper.ThrowArgumentNullIfNull(serviceName, "serviceName");
            ThrowHelper.ThrowArgumentOutOfRangeIfEmpty(serviceName, "serviceName");

            log.Debug(m => m("Allowing service '{0}' to interact with desktop...", serviceName));            

            RegistryKey registryKey = null;
            try
            {
                registryKey = Registry.LocalMachine.OpenSubKey(
                    string.Format(@"SYSTEM\CurrentControlSet\Services\{0}", serviceName), true);
                
                if (registryKey != null)
                {
                    object value = registryKey.GetValue("Type");
                    if ( value != null)
                    {
                        registryKey.SetValue("Type", ((int)value | 256));
                    }
                }
            }
            finally
            {
                if(registryKey != null)
                {
                    registryKey.Close();
                }
            }
            log.Debug(m => m("Done allowing service '{0}' to interact with desktop.", serviceName));
        }

        public static List<string> GetInstalledServices()
        {
            var returnValue = GetInstalledServices(s => true);
            return returnValue;
        }

        public static List<string> GetInstalledServices(
            Predicate<ServiceController> filter)
        {
            log.Debug(m => m("Getting list of installed services..."));
            
            var serviceControllers = Array.ConvertAll(
                ServiceController.GetServices(), s => s );
            
            var returnValue = new List<string>(
                Array.ConvertAll(
                    Array.FindAll(serviceControllers, filter),
                    s => s.ServiceName));
            log.Debug(m => m("Done getting list of installed services. Found '{0}' services.", returnValue.Count));            
            
            return returnValue;
        }

        private static SC_ACTION getScAction(ServiceRecoveryAction action,
            uint restartServiceAfter)
        {
            var scAction = new SC_ACTION();
            SC_ACTION_TYPE actionType = default(SC_ACTION_TYPE);
            switch (action)
            {
                case ServiceRecoveryAction.TakeNoAction:
                    actionType = SC_ACTION_TYPE.None;
                    break;
                case ServiceRecoveryAction.RestartTheService:
                    actionType = SC_ACTION_TYPE.RestartService;
                    break;
                case ServiceRecoveryAction.RestartTheComputer:
                    actionType = SC_ACTION_TYPE.RebootComputer;
                    break;
                case ServiceRecoveryAction.RunAProgram:
                    actionType = SC_ACTION_TYPE.RunCommand;
                    break;
            }
            scAction.Type = actionType;
            scAction.Delay = restartServiceAfter;
            return scAction;
        }

        private static ServiceRecoveryAction getServiceRecoveryAction(SC_ACTION action)
        {
            ServiceRecoveryAction serviceRecoveryAction = default(ServiceRecoveryAction);
            switch (action.Type)
            {
                case SC_ACTION_TYPE.None:
                    serviceRecoveryAction = ServiceRecoveryAction.TakeNoAction;
                    break;
                case SC_ACTION_TYPE.RestartService:
                    serviceRecoveryAction = ServiceRecoveryAction.RestartTheService;
                    break;
                case SC_ACTION_TYPE.RebootComputer:
                    serviceRecoveryAction = ServiceRecoveryAction.RestartTheComputer;
                    break;
                case SC_ACTION_TYPE.RunCommand:
                    serviceRecoveryAction = ServiceRecoveryAction.RunAProgram;
                    break;
            }
            return serviceRecoveryAction;
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]        
        private static void grantShutdownPrivileges()
        {
            log.Debug(m => m("Granting shutdown privileges to process user..."));            
            
            IntPtr tokenHandle = IntPtr.Zero;
            
            TOKEN_PRIVILEGES tkp = new TOKEN_PRIVILEGES();

            long luid = 0;
            int retLen = 0;

            try
            {
                IntPtr processHandle = Process.GetCurrentProcess().Handle;
                bool success = OpenProcessToken(processHandle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref tokenHandle);
                if (!success)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to open process token.");
                }

                LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref luid);

                tkp.PrivilegeCount = 1;
                tkp.Privileges.Luid = luid;
                tkp.Privileges.Attributes = SE_PRIVILEGE_ENABLED;

                success = AdjustTokenPrivileges(tokenHandle, false, ref tkp, 0, IntPtr.Zero, ref retLen);
                if (!success)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to shutdown priveleges.");
                }
            }
            finally
            {
                if (tokenHandle != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(tokenHandle);
                }
                log.Debug(m => m("Done granting shutdown privileges to process user."));                        
            }
        }
    }
}