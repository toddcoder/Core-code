using Core.Collections;
using Core.Configurations;
using Core.Internet.Smtp;
using Core.Monads;
using Core.Services.Loggers;
using Core.Services.Plugins;
using static Core.Monads.MonadFunctions;

namespace Core.Services;

public class Service
{
   protected static Optional<Service> fromConfiguration(Configuration configuration, Optional<IServiceMessage> _serviceMessage)
   {
      return
         from name in configuration.Result.String("name")
         from addressSetting in configuration.Result.Setting("address")
         from exceptionAddress in addressSetting.Deserialize<Address>()
         from typeManager in TypeManager.FromConfiguration(configuration)
         from serviceLogger in ServiceLogger.FromConfiguration(configuration)
         select new Service(configuration, name, exceptionAddress, typeManager, serviceLogger, _serviceMessage);
   }

   public static Optional<Service> FromConfiguration(Configuration configuration) => fromConfiguration(configuration, nil);

   public static Optional<Service> FromConfiguration(Configuration configuration, IServiceMessage serviceMessage)
   {
      return fromConfiguration(configuration, serviceMessage.Some());
   }

   protected Configuration configuration;
   protected string name;
   protected Address exceptionAddress;
   protected TypeManager typeManager;
   protected StringHash<Job> jobs;
   protected StringHash<Subscription> subscriptions;
   protected ServiceMessage serviceMessage;

   protected Service(Configuration configuration, string name, Address exceptionAddress, TypeManager typeManager, ServiceLogger serviceLogger,
      Optional<IServiceMessage> _serviceMessage)
   {
      this.configuration = configuration;
      this.name = name;
      this.exceptionAddress = exceptionAddress;
      this.typeManager = typeManager;

      serviceMessage = new ServiceMessage(name);
      serviceMessage.Add(serviceLogger);
      serviceMessage.Add(new NamedExceptions(exceptionAddress, name, $"{name} Exceptions", 5));
      if (_serviceMessage is (true, var serviceMessageValue))
      {
         serviceMessage.Add(serviceMessageValue);
      }

      jobs = new StringHash<Job>(true);
      subscriptions = new StringHash<Subscription>(true);
   }
}