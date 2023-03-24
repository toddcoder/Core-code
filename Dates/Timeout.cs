using System;
using Core.Dates.Now;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Dates;

[Obsolete("Use Working")]
public class Timeout
{
   public static implicit operator Timeout(TimeSpan timeoutPeriod) => new(timeoutPeriod);

   public static implicit operator bool(Timeout timeout) => !timeout.Expired;

   public static bool operator true(Timeout timeout) => !timeout.Expired;

   public static bool operator false(Timeout timeout) => timeout.Expired;

   protected TimeSpan timeoutPeriod;
   protected Optional<DateTime> _targetDateTime;

   public Timeout(TimeSpan timeoutPeriod)
   {
      this.timeoutPeriod = timeoutPeriod;
      _targetDateTime = nil;
   }

   public bool Expired
   {
      get
      {
         DateTime targetDateTime;
         if (_targetDateTime)
         {
            targetDateTime = _targetDateTime;
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

   public Optional<DateTime> TargetDateTime => _targetDateTime;
}