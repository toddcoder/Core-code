using System;

namespace Core.Assertions
{
   public class Constraint
   {
      public static Constraint Failing(string message) => new Constraint(() => false, message, false);

      public Constraint(Func<bool> condition, string message, bool not)
      {
         Condition = condition;
         Message = message.Replace("$not ", not ? "not " : "");
         Not = not;
      }

      public Func<bool> Condition { get; }

      public string Message { get; }

      public bool Not { get; }

      public bool IsTrue() => Condition() != Not;
   }
}