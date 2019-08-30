using System;

namespace Core.Assertion
{
   public abstract class StringAssertion : INotProvider
   {
      protected string value;

      protected StringAssertion(string value)
      {
         this.value = value;
      }

      public bool Not { get; set; }

      public abstract StringAssertion IsNull();

      public abstract StringAssertion IsEmpty();

      public abstract StringAssertion Contains(string substring);

      public abstract StringAssertion Contains(string substring, StringComparison stringComparison);

      public abstract StringAssertion StartsWith(string substring);

      public abstract StringAssertion EndsWith(string substring);

      public abstract StringAssertion Within(params string[] array);
   }
}