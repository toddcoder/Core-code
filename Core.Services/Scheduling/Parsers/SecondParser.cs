namespace Core.Services.Scheduling.Parsers;

public class SecondParser : Parser
{
   public override string Pattern => "^ '::' /(/d+); f";

   protected override ScheduleIncrement getIncrement(int[] values)
   {
      setTop(out second, values[0], Validator.UnitType.Second);
      return new ScheduleIncrement(0, 0, 0, 0, 0, second);
   }
}