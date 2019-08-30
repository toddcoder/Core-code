using System;
using Core.Booleans;

namespace Core.Assertion
{
   public class ThrowingStringAssertion : StringAssertion
   {
      public ThrowingStringAssertion(string value) : base(value) { }

      public override StringAssertion IsNull() => this;

      public override StringAssertion IsEmpty() => this;

      public override StringAssertion Contains(string substring) => this;

      public override StringAssertion Contains(string substring, StringComparison stringComparison) => this;

      public override StringAssertion StartsWith(string substring) => this;

      public override StringAssertion EndsWith(string substring) => this;

      public override StringAssertion Within(params string[] array) => this;
   }
}