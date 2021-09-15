using System;
using Core.Dates.Now;
using Core.Monads;
using Core.Services.Scheduling.Brackets;
using Core.Services.Scheduling.Parsers;

namespace Core.Services.Scheduling
{
   public class Schedule
   {
      public static implicit operator Schedule(string source) => new(source);

      protected ScheduleIncrement increment;
      protected Bracket bracket;
      protected DateTime targetDateTime;
      protected string schedule;
      protected bool autoNext;

      public Schedule(string schedule, bool autoNext = false)
      {
         this.schedule = schedule;
         this.autoNext = autoNext;

         var parser = new ScheduleParser();
         increment = parser.Parse(schedule).Required($"Didn't understand schedule {schedule}");
         bracket = parser.Bracket;
         targetDateTime = DateTime.MinValue;
      }

      public Bracket Bracket => bracket;

      public bool AutoNext
      {
         get => autoNext;
         set => autoNext = value;
      }

      public Result<DateTime> Next()
      {
         if (increment.Next.If(out var next, out var exception))
         {
            targetDateTime = next;
            return targetDateTime;
         }
         else
         {
            return exception;
         }
      }

      public bool Triggered
      {
         get
         {
            var now = NowServer.Now;
            var triggered = now >= targetDateTime;
            if (triggered && autoNext)
            {
               Next();
            }

            return bracket.Within(now) && triggered;
         }
      }

      public DateTime TargetDateTime => targetDateTime;

      public override string ToString() => schedule;
   }
}