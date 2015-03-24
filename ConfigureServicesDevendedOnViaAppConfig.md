# Configure the List of Services Depended On via App.Config #

Daemoniq allows users to specify the list of services that your windows service requires in order to function properly via the app.config file. This is enabled by adding a `servicesDependedOn` element inside the service definition. Once successfully installed, the list of `service` elements inside the `servicesDependedOn` will appear in the Service Dependencies tab in the services.msc applet.

[http://daemoniq.org](http://technet.microsoft.com/en-us/library/Bb742520.ssvc106_big(en-us,TechNet.10).gif)

The sample configuration file below illustrates this feature.

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
        displayName="Dummy Service "
        description="This is a dummy service"
        serviceStartMode="Manual">
        <servicesDependedOn>
          <service name="MSMQ" />
          <service name="TlntSvr" />
        </servicesDependedOn>        
      </service>
    </services>
  </daemoniq>
</configuration>
```