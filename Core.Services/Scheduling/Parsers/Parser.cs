using System.Linq;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Services.Scheduling.Brackets;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Services.Scheduling.Parsers;

public abstract class Parser
{
   protected const string PATTERN_BRACKET = "/s* /('[' /d+ ':' /d+ /s* '-' /s* /d+ ':' /d+ ']')? $; f";
   protected const string PATTERN_UNADORNED_BRACKET = "/s* '[' /(/d+ ':' /d+) /s* '-' /s* /(/d+ ':' /d+) ']'; f";

   protected int year;
   protected int month;
   protected int day;
   protected int hour;
   protected int minute;
   protected int second;
   protected Validator validator;
   protected string[] tokens;

   public Parser()
   {
      validator = new Validator();
      year = month = day = hour = minute = second = 0;
   }

   public abstract string Pattern { get; }

   public Maybe<ScheduleIncrement> Parse(string source)
   {
      var _tokens = source.Matches(Pattern + PATTERN_BRACKET).Map(r => r.Groups(0));
      if (_tokens)
      {
         tokens = _tokens.Value;
         var lastIndex = tokens.Length - 1;
         var values = tokens.Where((_, i) => i.Between(1).Until(lastIndex)).Select(t => Value.Int32(t, -1)).ToArray();
         var bracketSource = tokens[lastIndex];
         var _times = bracketSource.Matches(PATTERN_UNADORNED_BRACKET);
         if (_times)
         {
            var (begin, end) = _times.Value;
            Bracket = new LimitedBracket(begin, end);
         }
         else
         {
            Bracket = new AlwaysBracket();
         }

         return getIncrement(values);
      }

      return nil;
   }

   protected abstract ScheduleIncrement getIncrement(int[] values);

   public Bracket Bracket { get; set; }

   protected void setTop(out int target, int value, Validator.UnitType type)
   {
      target = value;
      validator.Type = type;
      validator.Test = t => t > 0;
      validator.Assert(target);
   }

   protected void setYear(int value)
   {
      year = value;
      validator.Type = Validator.UnitType.Year;
      validator.Test = y => y > 0;
      validator.Assert(year);
   }

   protected void setMonth(int value)
   {
      month = value;
      validator.Type = Validator.UnitType.Month;
      validator.Test = m => m.Between(1).And(12);
      validator.Assert(month);
   }

   protected void setDay(int value)
   {
      day = value;
      validator.Type = Validator.UnitType.Day;
      validator.Test = d => d.Between(0).And(31);
      validator.Assert(day);
   }

   protected void setHour(int value)
   {
      hour = value;
      validator.Type = Validator.UnitType.Hour;
      validator.Test = h => h.Between(0).Until(24);
      validator.Assert(hour);
   }

   protected void setMinute(int value)
   {
      minute = value;
      validator.Type = Validator.UnitType.Minute;
      validator.Test = m => m.Between(0).Until(60);
      validator.Assert(minute);
   }

   protected void setSecond(int value)
   {
      second = value;
      validator.Type = Validator.UnitType.Second;
      validator.Test = s => s.Between(0).Until(60);
      validator.Assert(second);
   }
}