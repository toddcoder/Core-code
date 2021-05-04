using System;
using Core.Monads;
using Core.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class ObjectTests
   {
      [TestMethod]
      public void IsNullTest()
      {
         (string, string[]) obj = (null, null);
         Console.WriteLine(obj.AnyNull() ? "Is null" : "Is not null");

         var maybe = obj.Some();
         Console.WriteLine(maybe.Map(t => t.Item1).DefaultTo(() => "none"));
      }
   }
}