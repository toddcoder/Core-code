using System;
using Core.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   class StringIntKey : EquatableBase
   {
      [Equatable]
      public int Index;

      public StringIntKey(string name, int index)
      {
         Name = name;
         Index = index;
      }

      [Equatable]
      public string Name { get; }
   }

   [TestClass]
   public class EquatableTest
   {
      [TestMethod]
      public void BasicTest()
      {
         var starting = new StringIntKey("foo", 153);
         var alike = new StringIntKey("foo", 153);
         var unalike = new StringIntKey("bar", 123);

         Console.WriteLine(starting.Equals(alike));
         Console.WriteLine(starting.Equals(unalike));
      }
   }
}
