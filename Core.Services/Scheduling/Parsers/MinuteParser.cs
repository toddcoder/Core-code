namespace Core.Services.Scheduling.Parsers;

public class MinuteParser : Parser
{
   public override string Pattern => "^ ':' /(/d+); f";

   protected override ScheduleIncrement getIncrement(int[] values)
   {
      setTop(out minute, values[0], Validator.UnitType.Minute);
      return new ScheduleIncrement(0, 0, 0, 0, minute, 0);
   }
}