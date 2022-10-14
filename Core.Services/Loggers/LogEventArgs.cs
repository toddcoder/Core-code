using System;
using Core.Computers;

namespace Core.Services.Loggers;

public class LogEventArgs : EventArgs
{
   protected FileName log;

   public LogEventArgs(FileName log)
   {
      this.log = log;
   }

   public FileName Log => log;
}