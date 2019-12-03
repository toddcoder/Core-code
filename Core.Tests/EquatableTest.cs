using Core.Assertions;
using Core.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   class StringIntKey : EquatableComparableBase
   {
      [Equatable, Comparable(1)]
      public int Index;

      public StringIntKey(string name, int index)
      {
         Name = name;
         Index = index;
      }

      [Equatable, Comparable(0)]
      public string Name { get; }
   }

   [TestClass]
   public class EquatableTest
   {
      [TestMethod]
      public void EqualityTest()
      {
         var starting = new StringIntKey("foo", 153);
         var alike = new StringIntKey("foo", 153);
         var unalike = new StringIntKey("bar", 123);

         starting.Must().Equal(alike);
         starting.Must().Not.Equal(unalike);
      }

      [TestMethod]
      public void ComparableTest()
      {
         var starting = new StringIntKey("foo", 123);
         var greaterThan = new StringIntKey("foo", 153);
         var lessThan = new StringIntKey("bar", 123);

         (starting<=greaterThan).Must().Be().Assert();
         (starting>=lessThan).Must().Be().Assert();
      }
   }
}
