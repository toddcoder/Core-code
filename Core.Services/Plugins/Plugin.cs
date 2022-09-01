using System;
using Core.Applications;
using Core.Configurations;
using Core.Internet.Smtp;
using Core.Monads;
using Core.Services.Loggers;
using Core.Services.Scheduling;
using Core.Strings;
using Standard.Services.Plugins;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Services.Plugins
{
   public abstract class Plugin
   {
      protected string name;
      protected Configuration configuration;
      protected Group jobGroup;
      protected ServiceMessage serviceMessage;
      protected Address address;
      protected int retries;
      protected string finalExceptionMessage;
      protected Maybe<Scheduler> _scheduler;
      protected Maybe<Retrier> _retrier;
      protected bool dispatchEnabled;
      protected string applicationName;

      public Plugin(string name, Configuration configuration, Group jobGroup)
      {
         this.name = name;
         this.configuration = configuration;
         this.jobGroup = jobGroup;

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
         if (_retrier.Map(out var r))
         {
            r.Execute();
            if (r.AllRetriesFailed)
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
            jobGroup.Fill(ref obj);

            createScheduler();

            var exceptionsGroup = jobGroup.GroupAt("exceptions");
            var exceptionsTitle = exceptionsGroup.ValueAt("title");

            if (exceptionsGroup.GroupAt("address").Deserialize<Address>().Map(out address, out var exception))
            {
               if (ServiceLogger.FromConfiguration(configuration).Map(out var serviceLogger, out exception))
               {
                  retries = jobGroup.GetValue("retries").Map(Maybe.Int32) | 0;
                  SetRetrier();
                  finalExceptionMessage = $"All {retries} {"retr(y|ies)".Plural(retries)} failed";

                  var namedExceptions = new NamedExceptions(address, name, exceptionsTitle, retries);

                  applicationName = configuration.ValueAt("name");

                  serviceMessage = new ServiceMessage(applicationName);
                  serviceMessage.Add(serviceLogger);
                  serviceMessage.Add(namedExceptions);

                  return unit;
               }
               else
               {
                  return exception;
               }
            }
            else
            {
               return exception;
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
         if (_retrier.Map(out var retrier))
         {
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

      protected static Maybe<Scheduler> getScheduler(string source) => maybe(source != "none", () => new Scheduler(source));

      protected virtual void createScheduler()
      {
         _scheduler = jobGroup.GetValue("schedule").Map(getScheduler);
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
}