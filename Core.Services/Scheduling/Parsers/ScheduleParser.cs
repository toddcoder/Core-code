using System.Collections.Generic;
using Core.Monads;
using Core.Services.Scheduling.Brackets;
using static Core.Monads.MonadFunctions;

namespace Core.Services.Scheduling.Parsers;

public class ScheduleParser
{
   protected List<Parser> parsers;

   public ScheduleParser() => parsers = new List<Parser>
   {
      new YearParser(),
      new MonthParser(),
      new DayParser(),
      new DayOfWeekParser(),
      new HourParser(),
      new MinuteParser(),
      new SecondParser()
   };

   public Maybe<ScheduleIncrement> Parse(string source)
   {
      foreach (var parser in parsers)
      {
         var increment = parser.Parse(source);
         if (increment)
         {
            Bracket = parser.Bracket;
            return increment;
         }
      }

      return nil;
   }

   public Bracket Bracket { get; set; }
}