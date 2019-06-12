using System;

namespace Core.Applications
{
	public class Retrier
	{
		int retries;
		Action<int> action;
		Action<Exception, int> exceptionAction;
		int attempt;

		public event EventHandler<RetryArgs> Successful;
		public event EventHandler<RetryArgs> Failed;

		public Retrier(int retries, Action<int> action, Action<Exception, int> exceptionAction)
		{
			this.retries = retries;
			this.action = action;
			this.exceptionAction = exceptionAction;
			attempt = 0;
		}

		public void Execute()
		{
			for (var retry = 0; retry <= retries; retry++)
			{
				try
				{
					action(retry);
					Successful?.Invoke(this, new RetryArgs(attempt));
					attempt = 0;
					return;
				}
				catch (Exception exception)
				{
					exceptionAction(exception, retry);
					attempt = retry;
				}

				Failed?.Invoke(this, new RetryArgs(attempt));
			}
		}

		public int Attempt => attempt;

		public bool Retried => attempt > 0;

		public bool AllRetriesFailed => attempt == retries;
	}
}