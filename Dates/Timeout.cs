using System;
using System.Threading;
using Core.Dates.DateIncrements;
using Core.Dates.Now;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Dates;

public class Timeout
{
   public static implicit operator Timeout(TimeSpan timeoutPeriod) => new(timeoutPeriod);

   protected TimeSpan timeoutPeriod;
   protected Maybe<DateTime> _targetDateTime;
   protected TimeSpan sleepPeriod;
   protected bool cancelled;
   protected bool wasCancelled;

   protected Timeout(TimeSpan timeoutPeriod)
   {
      this.timeoutPeriod = timeoutPeriod;

      _targetDateTime = nil;
      sleepPeriod = 500.Milliseconds();
      cancelled = false;
      wasCancelled = false;
   }

   public bool IsPending()
   {
      Thread.Sleep(sleepPeriod);

      if (cancelled)
      {
         cancelled = false;
         wasCancelled = true;

         return false;
      }

      if (!_targetDateTime)
      {
         _targetDateTime = NowServer.Now + timeoutPeriod;
      }

      if (_targetDateTime is (true, var targetDateTime))
      {
         var stillWorking = NowServer.Now <= targetDateTime;
         if (!stillWorking)
         {
            _targetDateTime = nil;
         }

         return stillWorking;
      }
      else
      {
         return false;
      }
   }

   public TimeSpan TimeoutPeriod => timeoutPeriod;

   public DateTime TargetDateTime => _targetDateTime | (() => NowServer.Now + timeoutPeriod);

   public TimeSpan SleepingPeriod
   {
      get => sleepPeriod;
      set => sleepPeriod = value;
   }

   public TimeSpan Elapsed => _targetDateTime.Map(t => t - DateTime.Now) | TimeSpan.Zero;

   public void Cancel() => cancelled = true;

   public bool WasCancelled => wasCancelled;
}