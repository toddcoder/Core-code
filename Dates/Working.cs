using System;
using System.Threading;
using Core.Dates.DateIncrements;
using Core.Dates.Now;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Dates;

public class Working
{
   public static implicit operator Working(TimeSpan workingPeriod) => new(workingPeriod);

   public static implicit operator bool(Working working) => working.isWorking();

   public static bool operator true(Working working) => working.isWorking();

   public static bool operator false(Working working) => !working.isWorking();

   public static bool operator !(Working working) => !working.isWorking();

   protected TimeSpan workingPeriod;
   protected Maybe<DateTime> _targetDateTime;
   protected TimeSpan sleepPeriod;

   protected Working(TimeSpan workingPeriod)
   {
      this.workingPeriod = workingPeriod;

      _targetDateTime = nil;
      sleepPeriod = 500.Milliseconds();
   }

   public bool isWorking()
   {
      Thread.Sleep(sleepPeriod);

      if (!_targetDateTime)
      {
         _targetDateTime = NowServer.Now + workingPeriod;
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

   public TimeSpan WorkingPeriod => workingPeriod;

   public DateTime TargetDateTime => _targetDateTime | (() => NowServer.Now + workingPeriod);

   public TimeSpan SleepingPeriod
   {
      get => sleepPeriod;
      set => sleepPeriod = value;
   }

   public TimeSpan Elapsed => _targetDateTime.Map(t => t - DateTime.Now) | TimeSpan.Zero;
}