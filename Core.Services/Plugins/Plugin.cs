using System;
using Core.Applications;
using Core.Collections;
using Core.Configurations;
using Core.Internet.Smtp;
using Core.Monads;
using Core.Services.Loggers;
using Core.Services.Scheduling;
using Standard.Services.Plugins;

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
		protected Scheduler scheduler;
		protected Maybe<Retrier> _retrier;
		protected bool dispatchEnabled;
		protected string applicationName;

		public Plugin(string name, Configuration configuration, Group jobGroup)
		{
			this.name = name;
			this.configuration = configuration;
			this.jobGroup = jobGroup;

			dispatchEnabled = true;
		}

		public string Name => name;

		public DateTime TriggerDate { get; set; }

		public DateTime TargetDate { get; set; }

		public virtual void Initialize() { }

		public virtual void Deinitialize() { }

		public abstract void Dispatch();

		public virtual void OnStart() { }

		public virtual void OnStop() { }

		public virtual void OnPause() { }

		public virtual void OnContinue() { }

		public virtual void AfterFirstScheduled(Schedule schedule) { }

		public void InnerDispatch()
		{
			if (_retrier.If(out var r))
			{
				r.Execute();
				if (r.AllRetriesFailed)
            {
               serviceMessage.EmitExceptionMessage(finalExceptionMessage);
            }
         }
		}

		public virtual void InnerDispatch(int retry) { }

		public virtual void SuccessfulInnerDispatch(int retry) { }

		public virtual void FailedInnerDispatch(int retry) { }

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
			
			/*object obj = this;
			jobGroup.Fill(ref obj);

			createSchedules();

			var exceptionsGraph = jobGroup["exceptions"];
			var exceptionsTitle = exceptionsGraph["title"].Value;

			if (exceptionsGraph["address"].Object<Address>().If(out address, out var exception)) { }
			else
         {
            throw exception;
         }

         var serviceLogger = new ServiceLogger(configuration, jobGroup);

			retries = jobGroup.FlatMap("retries", g => g.Value.ToInt(), 0);
			SetRetrier();
			finalExceptionMessage = $"All {retries} {(retries == 1 ? "retry" : "retries")} failed";

         var namedExceptions = new NamedExceptions(address, name, exceptionsTitle, retries);

			applicationName = configuration.RootGraph["name"].Value;

			serviceMessage = new ServiceMessage(applicationName);
			serviceMessage.Add(serviceLogger);
			serviceMessage.Add(namedExceptions);*/
		}

		public void SetRetrier()
		{
			_retrier = when(retries > 0, () => new Retrier(retries, InnerDispatch, retryException));
			if (_retrier.If(out var r))
			{
				r.Successful += (sender, e) => SuccessfulInnerDispatch(e.RetryCount);
				r.Failed += (sender, e) => FailedInnerDispatch(e.RetryCount);
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

		protected static Schedules.Schedules getSchedules(string source)
		{
			return source == "none" ? new NullSchedules() : new Schedules.Schedules(source);
		}

		protected virtual void createSchedules()
		{
			scheduler = jobGroup.If("schedule", out var schedule) ? getSchedules(schedule.Value) : new NullSchedules();
		}

		public Schedules.Schedules GetSchedules() => scheduler;

		public virtual void BeforeDispatch(Schedule schedule) { }

		public virtual void AfterDispatch(Schedule schedule) { }

		public virtual bool DispatchEnabled
		{
			get => dispatchEnabled;
			set => dispatchEnabled = value;
		}

		public virtual void TargetDateTimes(Schedules.Schedules jobSchedules) { }

		public IServiceMessage ServiceMessage => serviceMessage;

		public IMaybe<AfterPlugin> After { get; set; } = none<AfterPlugin>();

		public bool Tracing { get; set; }
	}
}