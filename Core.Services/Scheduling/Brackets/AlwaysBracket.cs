using System;

namespace Core.Services.Scheduling.Brackets
{
   public class AlwaysBracket : Bracket
   {
      public override bool Within(DateTime now) => true;
   }
}