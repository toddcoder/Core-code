using System;
using Core.Assertions;

namespace Core.Services.Scheduling
{
   public class Validator
   {
      public enum UnitType
      {
         Second,
         Minute,
         Hour,
         Day,
         Month,
         Year
      }

      public Validator() => Test = i => i > 0;

      public UnitType Type { get; set; }

      public Func<int, bool> Test { get; set; }

      public void Assert(int value)
      {
         Test(value).Must().BeTrue().OrThrow(() => $"Value {value} {Type.ToString().ToLower()} is out of range");
      }
   }
}