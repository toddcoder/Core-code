namespace Core.Services.Scheduling.Parsers;

public class YearParser : Parser
{
   public override string Pattern => "^ /(/d+) '-' /(/d+) '-' /(/d+) '.' /(/d+) ':' /(/d+); f";

   protected override ScheduleIncrement getIncrement(int[] values)
   {
      setYear(values[0]);
      setMonth(values[1]);
      setDay(values[2]);
      setHour(values[3]);
      setMinute(values[4]);

      return new ScheduleIncrement(year, month, day, hour, minute, 0);
   }
}