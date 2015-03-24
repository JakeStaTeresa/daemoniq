Developing and debugging windows services or daemon processes in .Net can be a really painful and tedious process. Daemoniq takes a stab at this problem by providing another level of a abstraction on top of System.ServiceProcess. This allows developers to concentrate on writing windows services in .Net by providing functionality such as configuration, deployment and debuggability.

Visit our [quickstart](http://code.google.com/p/daemoniq/wiki/Quickstart) section to know more about using Daemoniq.

Current features include:
  * Container agnostic service location via the [CommonServiceLocator](http://commonservicelocator.codeplex.com/)
  * [Set common service properties like serviceName, displayName, description and serviceStartMode via app.config](http://code.google.com/p/daemoniq/wiki/CommonServiceProperties)
  * [Run multiple windows services on the same process](http://code.google.com/p/daemoniq/wiki/MultipleWindowsServicesOneProcess)
  * [Set recovery options via app.config](http://code.google.com/p/daemoniq/wiki/WindowsServiceRecoveryOptions)
  * [Set services depended on via app.config](http://code.google.com/p/daemoniq/wiki/ConfigureServicesDevendedOnViaAppConfig)
  * set service process credentials via command-line
    * when using /credentials=user, you can opt to pass /password or /p instead of specifying a password. This will cause the program prompt you to enter your password in the console. Each character of your password will be masked to protect it from prying eyes.
  * install, uninstall, debug services via command-line