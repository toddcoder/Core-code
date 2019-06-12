using Core.Dates.Relative.DateOperations;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Dates.Relative.Parsers
{
   public class ThisParser : Parser
   {
      public override string Pattern => "^ /('this' | 'first' | 'last' | 'next') /s+ /('month' | 'year' | 'day')";

      public override IMatched<DateOperation> Parse(string source, string[] tokens)
      {
         var time = tokens[1].ToLower();
         var unit = tokens[2].ToLower();
         switch (time)
         {
            case "this":
               return new RelativeDay(0).Matched<DateOperation>();
            case "first":
               switch (unit)
               {
                  case "month":
                     return new AbsoluteMonth(1).Matched<DateOperation>();
                  case "year":
                     return new AbsoluteYear(1).Matched<DateOperation>();
                  case "day":
                     return new AbsoluteDay(1).Matched<DateOperation>();
               }

               break;
            case "last":
               switch (unit)
               {
                  case "month":
                     return new RelativeMonth(-1).Matched<DateOperation>();
                  case "year":
                     return new RelativeYear(-1).Matched<DateOperation>();
                  case "day":
                     return new LastDayOfMonth().Matched<DateOperation>();
               }

               break;
            case "next":
               switch (unit)
               {
                  case "month":
                     return new RelativeMonth(1).Matched<DateOperation>();
                  case "year":
                     return new RelativeYear(1).Matched<DateOperation>();
                  case "day":
                     return new RelativeDay(1).Matched<DateOperation>();
               }

               break;
            default:
               return notMatched<DateOperation>();
         }

         return notMatched<DateOperation>();
      }
   }
}