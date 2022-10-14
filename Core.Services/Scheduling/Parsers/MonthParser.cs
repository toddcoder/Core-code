namespace Core.Services.Scheduling.Parsers;

public class MonthParser : Parser
{
   public override string Pattern => "^ /(/d+) '-' /(/d+) '.' /(/d+) ':' /(/d+); f";

   protected override ScheduleIncrement getIncrement(int[] values)
   {
      setTop(out month, values[0], Validator.UnitType.Month);
      setDay(values[1]);
      setHour(values[2]);
      setMinute(values[3]);

      return new ScheduleIncrement(0, month, day, hour, minute, 0);
   }
}