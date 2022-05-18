using System;
using Core.Dates.Now;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Dates
{
   public class Timeout
   {
      public static implicit operator Timeout(TimeSpan timeoutPeriod) => new(timeoutPeriod);

      protected TimeSpan timeoutPeriod;
      protected Maybe<DateTime> _targetDateTime;

      public Timeout(TimeSpan timeoutPeriod)
      {
         this.timeoutPeriod = timeoutPeriod;
         _targetDateTime = nil;
      }

      public bool Expired
      {
         get
         {
            if (_targetDateTime.Map(out var targetDateTime))
            {
            }
            else
            {
               targetDateTime = NowServer.Now + timeoutPeriod;
               _targetDateTime = targetDateTime;
            }

            var expired = NowServer.Now >= targetDateTime;
            if (expired)
            {
               _targetDateTime = nil;
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
            _targetDateTime = nil;
         }
      }

      public Maybe<DateTime> TargetDateTime => _targetDateTime;
   }
}