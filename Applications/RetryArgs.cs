using System;

namespace Core.Applications;

public class RetryArgs : EventArgs
{
   protected int retryCount;

   public RetryArgs(int retryCount) => this.retryCount = retryCount;

   public int RetryCount => retryCount;
}