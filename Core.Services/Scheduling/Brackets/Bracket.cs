using System;

namespace Core.Services.Scheduling.Brackets
{
   public abstract class Bracket
   {
      public abstract bool Within(DateTime now);
   }
}