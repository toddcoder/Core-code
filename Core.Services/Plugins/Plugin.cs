using System;
using Core.Applications;
using Core.Configurations;
using Core.Internet.Smtp;
using Core.Monads;
using Core.Services.Loggers;
using Core.Services.Scheduling;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Services.Plugins;

public abstract class Plugin
{
   protected string name;
   protected Configuration configuration;
   protected Setting jobSetting;
   protected ServiceMessage serviceMessage;
   protected Address address;
   protected int retries;
   protected string finalExceptionMessage;
   protected Maybe<Scheduler> _scheduler;
   protected Maybe<Retrier> _retrier;
   protected bool dispatchEnabled;
   protected string applicationName;

   public Plugin(string name, Configuration configuration, Setting jobSetting)
   {
      this.name = name;
      this.configuration = configuration;
      this.jobSetting = jobSetting;

      dispatchEnabled = true;
      After = nil;
   }

   public string Name => name;

   public DateTime TriggerDate { get; set; }

   public DateTime TargetDate { get; set; }

   public virtual void Initialize()
   {
   }

   public virtual void Deinitialize()
   {
   }

   public abstract Result<Unit> Dispatch();

   public virtual void OnStart()
   {
   }

   public virtual void OnStop()
   {
   }

   public virtual void OnPause()
   {
   }

   public virtual void OnContinue()
   {
   }

   public virtual void AfterFirstScheduled(Schedule schedule)
   {
   }

   public void InnerDispatch()
   {
      if (_retrier)
      {
         var retrier = ~_retrier;
         retrier.Execute();
         if (retrier.AllRetriesFailed)
         {
            serviceMessage.EmitExceptionMessage(finalExceptionMessage);
         }
      }
   }

   public virtual void InnerDispatch(int retry)
   {
   }

   public virtual void SuccessfulInnerDispatch(int retry)
   {
   }

   public virtual void FailedInnerDispatch(int retry)
   {
   }

   protected void noRetryException(Exception exception, int retry)
   {
      serviceMessage.EmitException(new PluginException(this, exception));
   }

   protected void retryException(Exception exception, int retry)
   {
      serviceMessage.EmitExceptionAttempt(new PluginException(this, exception), retry);
   }

   public virtual Result<Unit> SetUp()
   {
      try
      {
         object obj = this;
         jobSetting.Fill(ref obj);

         createScheduler();

         var exceptionsSetting = jobSetting.Value.Setting("exceptions");
         var exceptionsTitle = exceptionsSetting.Value.String("title");

         var _address = exceptionsSetting.Value.Setting("address").Deserialize<Address>();
         if (_address)
         {
            address = _address;
            var _serviceLogger = ServiceLogger.FromConfiguration(configuration);
            if (_serviceLogger)
            {
               retries = jobSetting.Maybe.Int32("retries") | 0;
               SetRetrier();
               finalExceptionMessage = $"All {retries} {"retr(y|ies)".Plural(retries)} failed";

               var namedExceptions = new NamedExceptions(address, name, exceptionsTitle, retries);

               applicationName = configuration.Value.String("name");

               serviceMessage = new ServiceMessage(applicationName);
               serviceMessage.Add(~_serviceLogger);
               serviceMessage.Add(namedExceptions);

               return unit;
            }
            else
            {
               return _serviceLogger.Exception;
            }
         }
         else
         {
            return _address.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public void SetRetrier()
   {
      _retrier = maybe(retries > 0, () => new Retrier(retries, InnerDispatch, retryException));
      if (_retrier)
      {
         var retrier = ~_retrier;
         retrier.Successful += (_, e) => SuccessfulInnerDispatch(e.RetryCount);
         retrier.Failed += (_, e) => FailedInnerDispatch(e.RetryCount);
      }
   }

   public void SetRetrier(int retries)
   {
      this.retries = retries;
      SetRetrier();
   }

   public void SetDefaultRetries(int retries)
   {
      if (retries < 0)
      {
         SetRetrier(retries);
      }
   }

   protected static Maybe<Scheduler> getScheduler(string source) => maybe<Scheduler>() & source != "none" & (() => new Scheduler(source));

   protected virtual void createScheduler()
   {
      _scheduler = jobSetting.Maybe.String("schedule").Map(getScheduler);
   }

   public Maybe<Scheduler> Scheduler() => _scheduler;

   public virtual void BeforeDispatch(Schedule schedule)
   {
   }

   public virtual void AfterDispatch(Schedule schedule)
   {
   }

   public virtual bool DispatchEnabled
   {
      get => dispatchEnabled;
      set => dispatchEnabled = value;
   }

   public virtual void TargetDateTimes(Scheduler jobScheduler)
   {
   }

   public IServiceMessage ServiceMessage => serviceMessage;

   public Maybe<AfterPlugin> After { get; set; }

   public bool Tracing { get; set; }
}