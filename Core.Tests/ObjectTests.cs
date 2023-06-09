using System;
using Core.Monads;
using Core.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Objects.GetHashCodeGenerator;

namespace Core.Tests;

[TestClass]
public class ObjectTests
{
   [TestMethod]
   public void IsNullTest()
   {
      (string, string[]) obj = (null, null);
      Console.WriteLine(obj.AnyNull() ? "Is null" : "Is not null");

      var maybe = obj.Some();
      Console.WriteLine(maybe.Map(t => t.Item1) | "none");
   }

   [TestMethod]
   public void GetHashCodeTest()
   {
      int hash = hashCode() + 153 + "foobar" + true;
      Console.WriteLine(hash);
      int hash2 = hashCode() + 153 + "foobar" + true;
      Assert.AreEqual(hash, hash2);
      hash2 = hashCode() + 154 + "foobaz" + false;
      Console.WriteLine(hash2);
   }

   [TestMethod]
   public void LateLazyTest()
   {
      var i = new LateLazy<int>();
      i.ActivateWith(() => 153);
      Console.WriteLine(i * 2);

      i.Reset();
      Console.WriteLine(i * 10 + 4);
   }
}