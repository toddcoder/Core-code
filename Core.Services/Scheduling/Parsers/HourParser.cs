namespace Core.Services.Scheduling.Parsers;

public class HourParser : Parser
{
   public override string Pattern => "^ /(/d+) ':' /(/d+); f";

   protected override ScheduleIncrement getIncrement(int[] values)
   {
      setTop(out hour, values[0], Validator.UnitType.Hour);
      setMinute(values[1]);

      return new ScheduleIncrement(0, 0, 0, hour, minute, 0);
   }
}