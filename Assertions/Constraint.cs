using System;
using Core.Strings;

namespace Core.Assertions;

public class Constraint
{
   public static Constraint Failing(string message, string name) => new Constraint(() => false, message, false, name, new object());

   public static Constraint Formatted<T>(Func<bool> condition, string message, bool not, string name, T value, Func<T, string> formatter)
   {
      var newValue = formatter(value);
      return new Constraint(condition, message, not, name, newValue);
   }

   public static Constraint Formatted(Func<bool> condition, string message, bool not, string name)
   {
      return new Constraint(condition, message, not, name);
   }

   public Constraint(Func<bool> condition, string message, bool not, string name, object value)
   {
      Condition = condition;
      var valueImage = value.ToNonNullString();
      var newMessage = message.ReplaceAll(("$not ", not ? "not " : ""), ("$name", name), ("$value", valueImage));
      Not = not;

      if (value != null)
      {
         newMessage = $"{newMessage}; found value {valueImage}";
      }

      Message = newMessage;
   }

   public Constraint(Func<bool> condition, string message, bool not, string name)
   {
      Condition = condition;
      Message = message.ReplaceAll(("$not ", not ? "not " : ""), ("$name", name));
      Not = not;
   }

   public Func<bool> Condition { get; }

   public string Message { get; }

   public bool Not { get; }

   public bool IsTrue() => Condition() != Not;
}