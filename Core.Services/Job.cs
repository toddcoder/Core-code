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
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Services;

public class Job : IDisposable, IEquatable<Job>, IAddServiceMessages
{
   public static Result<Job> New(Setting jobSetting, TypeManager typeManager, Configuration configuration)
   {
      return
         from serviceLogger in ServiceLogger.FromConfiguration(configuration)
         let job = new Job(jobSetting, configuration, serviceLogger)
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
   protected Setting jobSetting;
   protected Configuration configuration;
   protected bool canStop;
   protected Maybe<string> _subscription;
   protected ServiceMessage serviceMessage;

   protected Job(Setting jobSetting, Configuration configuration, ServiceLogger serviceLogger)
   {
      this.jobSetting = jobSetting;
      name = this.jobSetting.Value.String("name");
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

   protected static Result<(string assemblyName, string typeName)> typeInfo(TypeManager typeManager, Setting setting)
   {
      Result<string> getValue(string objectName, Func<Maybe<string>> defaultValue, string message)
      {
         var _nameFromObjectName = lazy.maybe<string>();
         var _nameFromDefaultValue = lazy.maybe<string>();
         if (_nameFromObjectName.ValueOf(setting.Maybe.String(objectName)) is (true, var nameFromObjectName))
         {
            return nameFromObjectName;
         }
         else if (_nameFromDefaultValue.ValueOf(defaultValue()) is (true, var nameFromDefaultValue))
         {
            return nameFromDefaultValue;
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
         from typeInfo in typeInfo(typeManager, jobSetting)
         from pluginType in typeManager.Type(typeInfo.assemblyName, typeInfo.typeName)
         from createdPlugin in pluginType.TryCreate(name, configuration, jobSetting).CastAs<Plugin>()
         select createdPlugin;
      if (_plugin)
      {
         plugin = _plugin;
         if (plugin is IRequiresTypeManager requiresTypeManager)
         {
            requiresTypeManager.TypeManager = typeManager;
         }

         serviceMessage.EmitMessage($"Setting up {name} plugin");

         var _result = plugin.SetUp();
         if (!_result)
         {
            return _result.Exception;
         }

         _subscription = jobSetting.Maybe.String("subscription");
         if (!_subscription)
         {
            _scheduler = plugin.Scheduler();
         }

         interval = jobSetting.Maybe.String("interval").Map(Value.TimeSpan) | (() => 1.Second());

         plugin.After = jobSetting.Maybe.Setting("after").Map(afterSetting =>
         {
            var _afterPlugin =
               from afterName in afterSetting.Result.String("name")
               from typeInfo in typeInfo(typeManager, afterSetting)
               from afterPluginType in typeManager.Type(typeInfo.assemblyName, typeInfo.typeName)
               from afterPlugin in afterPluginType.TryCreate(afterName, configuration, afterSetting, jobSetting).CastAs<AfterPlugin>()
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
         return _plugin.Exception;
      }
   }

   public bool Enabled { get; set; }

   public bool Subscribing => _subscription;

   protected void enableTimer(bool timerEnabled)
   {
      if (_timer is (true, var timer))
      {
         _stopTime = maybe<DateTime>() & timerEnabled & (() => NowServer.Now);
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

         if (_stopTime && e.SignalTime > _stopTime)
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
               if (plugin.After is (true, var pluginAfter))
               {
                  pluginAfter.AfterFailure(exception);
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
      if (_scheduler is (true, var scheduler))
      {
         var schedule = scheduler.NextSchedule;
         plugin.BeforeDispatch(schedule);
         scheduler.Next();
         plugin.TargetDateTimes(scheduler);
         if (plugin.DispatchEnabled)
         {
            var _afterPlugin = plugin.After;
            var _dispatched = plugin.Dispatch();
            if (_dispatched)
            {
               if (_afterPlugin is (true, var afterPlugin))
               {
                  afterPlugin.AfterSuccess();
               }
            }
            else if (_afterPlugin is (true, var afterPlugin))
            {
               afterPlugin.AfterFailure(_dispatched.Exception);
            }
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
      var _dispatched = plugin.Dispatch();
      if (_dispatched)
      {
         serviceMessage.EmitMessage("Plugin dispatched");
      }
      else
      {
         serviceMessage.EmitException(_dispatched.Exception);
      }
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