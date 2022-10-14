namespace Core.Services.Scheduling.Parsers;

public class DayParser : Parser
{
   public override string Pattern => "^ /(/d+) '.' /(/d+) ':' /(/d+); f";

   protected override ScheduleIncrement getIncrement(int[] values)
   {
      setTop(out day, values[0], Validator.UnitType.Day);
      setHour(values[1]);
      setMinute(values[2]);

      return new ScheduleIncrement(0, 0, day, hour, minute, 0);
   }
}