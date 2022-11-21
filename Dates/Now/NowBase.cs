using System;

namespace Core.Dates.Now;

public abstract class NowBase
{
   public abstract DateTime Now { get; }

   public abstract DateTime Today { get; }
}