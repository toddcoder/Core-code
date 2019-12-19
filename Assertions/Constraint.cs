using System;

namespace Core.Assertions
{
   public class Constraint
   {
      public static Constraint Failing(string message, string name) => new Constraint(() => false, message, false, name);

      public Constraint(Func<bool> condition, string message, bool not, string name)
      {
         Condition = condition;
         Message = message.Replace("$not ", not ? "not " : "").Replace("$name", name);
         Not = not;
      }

      public Func<bool> Condition { get; }

      public string Message { get; }

      public bool Not { get; }

      public bool IsTrue() => Condition() != Not;
   }
}