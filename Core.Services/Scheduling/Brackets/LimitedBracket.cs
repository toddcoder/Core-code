using System;
using Core.Dates;
using Core.Numbers;

namespace Core.Services.Scheduling.Brackets;

public class LimitedBracket : Bracket
{
   protected const string TIME_BEFORE_MIDNIGHT = "23:59:59";

   protected Time begin;
   protected Time end;
   protected bool normal;

   public LimitedBracket(Time begin, Time end)
   {
      this.begin = begin;
      this.end = end;
      normal = this.begin < this.end;
   }

   public override bool Within(DateTime now)
   {
      if (normal)
      {
         return ((Time)now).Between(begin).And(end);
      }
      else
      {
         return now.Between(now + begin).And(now + (Time)TIME_BEFORE_MIDNIGHT) || now.Between(now.Truncate()).And(now + end);
      }
   }
}