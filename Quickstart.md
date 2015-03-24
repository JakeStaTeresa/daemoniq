# Getting Started #

Download the source code or binaries from the **[downloads](http://code.google.com/p/daemoniq/downloads/list)** page. When compiling from source code, the build scripts are located in the build/scripts/ directory. There you will find several batch files and a nant.build file. Execute full-build.bat to fully compile Daemoniq.

Daemoniq uses the [Common Service Locator](http://commonservicelocator.codeplex.com/) from Microsoft Patterns and Practices to locate the different service instances. In this example, we will use the [Castle Windsor Adapter](http://commonservicelocator.codeplex.com/wikipage?title=Castle%20Windsor%20Adapter&referringTitle=Home) to setup our project. Because of this you will need to download the binaries for these projects.

Once you get your hands on the necessary binaries, you can create a console application project named `DummyApplication` in your favorite ide. Add a reference to  Daemoniq.dll in this project. Also please add a reference to the binaries from [CommonServiceLocator.WindsorAdapter](http://commonservicelocator.codeplex.com/Project/Download/FileDownload.aspx?DownloadId=44827) and [Microsoft.Patterns.ServiceLocation](http://commonservicelocator.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=17694).

_Note: you don't need to have a reference to System.ServiceProcess to create windows services in Daemoniq._

# Implementing the Service #

For illustration purposes, we will create a service that launches a thread when started and waits for a stop signal in the secondary thread. When stopped, the service will send the stop signal to the secondary thread which will cause it to stop waiting and exit. We will call this service _DummyService_.

Below is the sample code for the DummyService class.

```
using System.Diagnostics;
using System.Threading;
using Daemoniq.Framework;

namespace Daemoniq.Samples
{
    public class DummyService:ServiceInstanceBase
    {
        private readonly ManualResetEvent stopRequested = new ManualResetEvent(false);        

        public override void OnStart()
        {
            var t = new Thread(threadProc);
            t.Start();
        }

        public override void OnStop()
        {
            stopRequested.Set();
        }

        private void threadProc()
        {
            //service implementation code goes here.
            stopRequested.WaitOne();
        }   
    }
}
```

# Initializing the Service Application #

Now that we have the necessary references, make sure to call `ServiceLocator.SetLocatorProvider` method and pass in a delegate to a method which returns the `WindsorServiceLocator`.

Below is a sample code for initializing our `DummyApplication`. It initializes `WindsorContainer` with an `XmlInterpreter` because we will be setting up the components via the config file.

```
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CommonServiceLocator.WindsorAdapter;
using Daemoniq.Framework;
using Microsoft.Practices.ServiceLocation;

namespace Daemoniq.Samples
{
    public class DummyApplication
    {
        public static void Main(string[] args)
        {
            ServiceLocator.SetLocatorProvider(createWindsorContainer);
            args = new string[]{"/action=debug"};
            new ServiceApplication().Run(args);
        }

        private static WindsorServiceLocator createWindsorContainer()
        {
            return new WindsorServiceLocator(new WindsorContainer(new XmlInterpreter()));
        }
    }
}
```

# Configuring the Service #

Below is a sample code that:
  * registers the component to the `WindsorContainer`
  * refer to the component in the service definitions section of the `DaemoniqConfigurationSection`.
  * specify the serviceName, displayName and description of the service. These parameters will appear in the services.msc applet.

_Note that serviceName and component id should be identical in order for Daemoniq to locate the services successfully_

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="daemoniq" type="Daemoniq.Configuration.DaemoniqConfigurationSection, Daemoniq" />
    <section name="castle" type="Castle.Windsor.Configuration.AppDomain.CastleSectionHandler, Castle.Windsor" />
  </configSections>
  <castle>
    <components>
      <component id="Dummy"
                 service="Daemoniq.Framework.IServiceInstance, Daemoniq"
                 type="Daemoniq.Samples.DummyService, Daemoniq.Samples">
      </component>      
    </components>
  </castle>
  <daemoniq>
    <services>
      <service
        serviceName="Dummy"
        displayName="Dummy Service"
        description="This is a dummy service"
        serviceStartMode="Manual">
      </service>
    </services>
  </daemoniq>
</configuration>
```

That's it! We can now compile our project and try to install/uninstall and/or debug it.

# Installing the Service #

To install the service, from the command line, enter `DummyApplication.exe /action=install`.

By default, the service will be installed using the 'localSystem' credentials. To know more about the different credential types you can use, enter `DummyApplication.exe /help` or just enter `DummyApplication.exe` without any parameters.

Specifying this command-line argument/s causes your application to be registered in the services.msc applet.

# Uninstalling the Service #

To uninstall the service from the command line, enter `DummyApplication.exe /action=uninstall`

Specifying this command-line argument/s causes your application to be removed from the services.msc applet.

# Debugging the Service #
To debug the service from the command line, enter `DummyApplication.exe /action=debug`

Specifying this command-line argument/s launches your application as a console application.