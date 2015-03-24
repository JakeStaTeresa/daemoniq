# Hosting Multiple Windows Services in One Process #

Daemoniq allows users to run multiple services in one process. This can be done by registering two services in the IOC container and registering two service definitions in the app.config.

The sample configuration file below illustrates hosting 2 instances of the same service (each one with a different serviceName) in one process.

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="daemoniq" type="Daemoniq.Configuration.DaemoniqConfigurationSection, Daemoniq" />
    <section name="castle" type="Castle.Windsor.Configuration.AppDomain.CastleSectionHandler, Castle.Windsor" />
  </configSections>
  <castle>
    <components>
      <component id="Dummy1"
                 service="Daemoniq.Framework.IServiceInstance, Daemoniq"
                 type="Daemoniq.Samples.DummyService, Daemoniq.Samples">
      </component>
     <component id="Dummy2"
                 service="Daemoniq.Framework.IServiceInstance, Daemoniq"
                 type="Daemoniq.Samples.DummyService, Daemoniq.Samples">
      </component>      
    </components>
  </castle>
  <daemoniq>
    <services>
      <service
        serviceName="Dummy1"
        displayName="Dummy Service 1"
        description="This is a dummy service"
        serviceStartMode="Manual">
      </service>
      <service
        serviceName="Dummy2"
        displayName="Dummy Service 2"
        description="This is a dummy service"
        serviceStartMode="Manual">
      </service>
    </services>
  </daemoniq>
</configuration>
```

_Note: This feature can also be used to host two or more instances of different services in one process_