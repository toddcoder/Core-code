using System;
using System.Threading;
using Core.Applications.Writers;
using Core.Objects;

namespace Core.Applications
{
	public abstract class CommandLine : IDisposable
	{
		static IWriter getStandardWriter() => new ConsoleWriter();

		static IWriter getExceptionWriter() => new ConsoleWriter
		{
			ForegroundColor = ConsoleColor.Red,
			BackgroundColor = ConsoleColor.White
		};

		protected ManualResetEvent resetEvent;
		protected bool threading;

		public CommandLine(bool threading = false) : this(getStandardWriter(), threading) { }

		public CommandLine(IWriter standardWriter, bool threading = false) : this(standardWriter, getExceptionWriter(), threading) { }

		public CommandLine(IWriter standardWriter, IWriter exceptionWriter, bool threading = false)
		{
			StandardWriter = standardWriter;
			ExceptionWriter = exceptionWriter;
			Test = false;
			Running = true;
			this.threading = threading;
			if (threading)
			{
				resetEvent = new ManualResetEvent(false);
				Console.CancelKeyPress += (sender, e) =>
				{
					resetEvent.Set();
					e.Cancel = true;
				};
			}
		}

		public void Wait()
		{
			if (threading)
				resetEvent.WaitOne();
		}

		public IWriter StandardWriter { get; set; }

		public IWriter ExceptionWriter { get; set; }

		public bool Test { get; set; }

		public bool Running { get; set; }

		public abstract void Execute(Arguments arguments);

		public virtual void HandleException(Exception exception) => ExceptionWriter.WriteExceptionLine(exception);

		public virtual void Deinitialize() { }

		public virtual void Run(string[] args)
		{
			var arguments = new Arguments(args);

			try
			{
				Execute(arguments);
			}
			catch (Exception exception)
			{
				HandleException(exception);
			}

			if (Test)
				Console.ReadLine();
		}

		public virtual void RunInLoop(string[] args, TimeSpan interval)
		{
			var arguments = new Arguments(args);

			try
			{
				while (Running)
				{
					Execute(arguments);
					Thread.Sleep(interval);
				}
			}
			catch (Exception ex)
			{
				HandleException(ex);
			}

			if (Test)
				Console.ReadLine();
		}

		void dispose()
		{
			StandardWriter?.DisposeIfDisposable();
			ExceptionWriter?.DisposeIfDisposable();
		}

		public void Dispose()
		{
			dispose();
			GC.SuppressFinalize(this);
		}

		~CommandLine() => dispose();
	}
}