using System;
using System.Timers;
using Core.Configurations;
using Core.Dates.DateIncrements;
using Core.Dates.Now;
using Core.Monads;
using Core.Objects;
using Core.Services.Loggers;
using Core.Services.Plugins;
using Core.Services.Scheduling;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Services
{
   public class Job : IDisposable, IEquatable<Job>, IAddServiceMessages
   {
      public static Result<Job> New(Group jobGroup, TypeManager typeManager, Configuration configuration)
      {
         return
            from serviceLogger in ServiceLogger.FromConfiguration(configuration)
            let job = new Job(jobGroup, configuration, serviceLogger)
            from loaded in job.Load(typeManager)
            select job;
      }

      protected Plugin plugin;
      protected Maybe<Timer> _timer;
      protected Maybe<DateTime> _stopTime;
      protected TimeSpan interval;
      protected bool stopped;
      protected Maybe<Scheduler> _scheduler;
      protected string name;
      protected Group jobGroup;
      protected Configuration configuration;
      protected bool canStop;
      protected Maybe<string> _subscription;
      protected ServiceMessage serviceMessage;

      protected Job(Group jobGroup, Configuration configuration, ServiceLogger serviceLogger)
      {
         this.jobGroup = jobGroup;
         name = this.jobGroup.ValueAt("name");
         this.configuration = configuration;

         serviceMessage = new ServiceMessage(name);
         serviceMessage.Add(serviceLogger);

         Enabled = false;
         stopped = false;
         _scheduler = nil;
         _timer = nil;
         canStop = false;
         _subscription = nil;
      }

      protected static Result<(string assemblyName, string typeName)> typeInfo(TypeManager typeManager, Group group)
      {
         Result<string> getValue(string objectName, Func<Maybe<string>> defaultValue, string message)
         {
            if (group.GetValue(objectName).If(out var name))
            {
               return name;
            }
            else if (defaultValue().If(out name))
            {
               return name;
            }
            else
            {
               return fail(message);
            }
         }

         Result<string> getAssemblyName() => getValue("assembly", () => typeManager.DefaultAssemblyName, "No default assembly name specified");

         Result<string> getTypeName() => getValue("type", () => typeManager.DefaultTypeName, "No default type name specified");

         return
            from assemblyName in getAssemblyName()
            from typeName in getTypeName()
            select (assemblyName, typeName);
      }

      public Result<Unit> Load(TypeManager typeManager)
      {
         var _plugin =
            from typeInfo in typeInfo(typeManager, jobGroup)
            from pluginType in typeManager.Type(typeInfo.assemblyName, typeInfo.typeName)
            from createdPlugin in pluginType.TryCreate(name, configuration, jobGroup).CastAs<Plugin>()
            select createdPlugin;
         if (_plugin.If(out plugin, out var exception))
         {
            if (plugin is IRequiresTypeManager requiresTypeManager)
            {
               requiresTypeManager.TypeManager = typeManager;
            }

            serviceMessage.EmitMessage($"Setting up {name} plugin");

            if (plugin.SetUp().IfNot(out exception))
            {
               return exception;
            }

            _subscription = jobGroup.GetValue("subscription");
            if (_subscription.IsNone)
            {
               _scheduler = plugin.Scheduler();
            }

            interval = jobGroup.GetValue("interval").Map(Value.TimeSpan).DefaultTo(() => 1.Second());

            plugin.After = jobGroup.GetGroup("after").Map(afterGroup =>
            {
               var _afterPlugin =
                  from afterName in afterGroup.RequireValue("name")
                  from typeInfo in typeInfo(typeManager, afterGroup)
                  from afterPluginType in typeManager.Type(typeInfo.assemblyName, typeInfo.typeName)
                  from afterPlugin in afterPluginType.TryCreate(afterName, configuration, afterGroup, jobGroup).CastAs<AfterPlugin>()
                  from setUp in afterPlugin.SetUp()
                  select afterPlugin;
               return _afterPlugin.Maybe();
            });

            Enabled = false;
            stopped = false;
            canStop = true;

            return unit;
         }
         else
         {
            return exception;
         }
      }

      public bool Enabled { get; set; }

      public bool Subscribing => _subscription.IsSome;

      protected void enableTimer(bool timerEnabled)
      {
         if (_timer.If(out var timer))
         {
            _stopTime = maybe(timerEnabled, () => NowServer.Now);
            timer.Enabled = timerEnabled;
         }
      }

      protected void trace(Func<string> message)
      {
         if (plugin.Tracing)
         {
            plugin.ServiceMessage.EmitMessage($"trace: {message()}");
         }
      }

      protected void onStart(object sender, ElapsedEventArgs e)
      {
         if (Enabled)
         {
            enableTimer(false);

            if (_stopTime.If(out var stopTime) && e.SignalTime > stopTime)
            {
               trace(() => "Stopped");
            }
            else
            {
               try
               {
               }
               catch (Exception exception)
               {
                  plugin.ServiceMessage.EmitException(exception);
                  if (plugin.After.If(out var afterPlugin))
                  {
                     afterPlugin.AfterFailure(exception);
                  }
               }
               finally
               {
                  enableTimer(true);
               }
            }
         }
         else
         {
            trace(() => "Not enabled");
         }
      }

      public void TriggerDispatch()
      {
         if (_scheduler.If(out var scheduler))
         {
            var schedule = scheduler.NextSchedule;
            plugin.BeforeDispatch(schedule);
            scheduler.Next();
            plugin.TargetDateTimes(scheduler);
            if (plugin.DispatchEnabled)
            {
               var _afterPlugin = plugin.After;
               plugin.Dispatch()
                  .OnSuccess(_ => _afterPlugin.IfThen(afterPlugin => afterPlugin.AfterSuccess()))
                  .OnFailure(exception => _afterPlugin.IfThen(afterPlugin => afterPlugin.AfterFailure(exception)));
            }

            plugin.AfterDispatch(schedule);
         }
      }

      public void OnStart()
      {
         serviceMessage.EmitMessage($"Starting {name}");

         plugin.Initialize();

         var timer = new Timer(interval.TotalMilliseconds);
         timer.Elapsed += onStart;
         _timer = timer;

         plugin.OnStart();
         Enabled = true;

         enableTimer(true);
      }

      public void Prepare()
      {
         plugin.Initialize();
         plugin.OnStart();
      }

      protected void stopTimer()
      {
         enableTimer(false);
         _timer.IfThen(timer => timer.Dispose());
      }

      public void OnStop()
      {
         serviceMessage.EmitMessage($"Stopping {name}");
         if (canStop)
         {
            plugin.OnStop();

            stopTimer();

            plugin.Deinitialize();

            stopped = true;
         }
         else
         {
            stopTimer();
            stopped = true;
         }
      }

      public void ExecutePlugin()
      {
         plugin.Dispatch()
            .OnSuccess(_ => serviceMessage.EmitMessage("Plugin dispatched"))
            .OnFailure(exception => serviceMessage.EmitException(exception));
      }

      public void OnPause() => plugin.OnPause();

      public void OnContinue() => plugin.OnContinue();

      public void Dispose()
      {
         if (!stopped)
         {
            OnStop();
         }
      }

      public bool Equals(Job other) => name == other.name;

      public void AddServiceMessages(params IServiceMessage[] serviceMessages)
      {
         if (plugin is IAddServiceMessages addServiceMessages)
         {
            addServiceMessages.AddServiceMessages(serviceMessages);
         }
      }
   }
}