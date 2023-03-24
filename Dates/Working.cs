using System;
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
   protected Optional<DateTime> _targetDateTime;

   protected Working(TimeSpan workingPeriod)
   {
      this.workingPeriod = workingPeriod;

      _targetDateTime = nil;
   }

   public bool isWorking()
   {
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

   public TimeSpan Elapsed => _targetDateTime.Map(t => t - DateTime.Now) | TimeSpan.Zero;
}