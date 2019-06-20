using System;
using Core.Dates.Now;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Dates
{
   public class Timeout
   {
      public static implicit operator Timeout(TimeSpan timeoutPeriod) => new Timeout(timeoutPeriod);

      TimeSpan timeoutPeriod;
      IMaybe<DateTime> targetDateTime;

      public Timeout(TimeSpan timeoutPeriod)
      {
         this.timeoutPeriod = timeoutPeriod;
         targetDateTime = none<DateTime>();
      }

      public bool Expired
      {
         get
         {
            if (targetDateTime.If(out var tdt)) { }
            else
            {
               tdt = NowServer.Now + timeoutPeriod;
               targetDateTime = tdt.Some();
            }

            var expired = NowServer.Now >= tdt;
            if (expired)
            {
               targetDateTime = none<DateTime>();
            }

            return expired;
         }
      }

      public TimeSpan TimeoutPeriod
      {
         get => timeoutPeriod;
         set
         {
            timeoutPeriod = value;
            targetDateTime = none<DateTime>();
         }
      }

      public IMaybe<DateTime> TargetDateTime => targetDateTime;
   }
}