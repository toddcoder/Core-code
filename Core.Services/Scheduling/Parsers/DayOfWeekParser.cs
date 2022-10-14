using System;

namespace Core.Services.Scheduling.Parsers;

public class DayOfWeekParser : Parser
{
   public override string Pattern => "^ /('sun' | 'mon' | 'tue' | 'wed' | 'thu' | 'fri' | 'sat') '.' /(/d+) ':' /(/d+); f";

   protected override ScheduleIncrement getIncrement(int[] values)
   {
      var dayOfWeek = tokens[1] switch
      {
         "mon" => DayOfWeek.Monday,
         "tue" => DayOfWeek.Tuesday,
         "wed" => DayOfWeek.Wednesday,
         "thu" => DayOfWeek.Thursday,
         "fri" => DayOfWeek.Friday,
         "sat" => DayOfWeek.Saturday,
         _ => DayOfWeek.Sunday
      };

      setHour(values[1]);
      setMinute(values[2]);

      return new ScheduleIncrement(0, 0, dayOfWeek, hour, minute, 0);
   }
}