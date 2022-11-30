using System;
using Core.Arrays;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

namespace Core.Tests;

[TestClass]
public class ArrayExtensionTests
{
   [TestMethod]
   public void AndifyTest()
   {
      var oneItem = array("See no evil");
      var twoItems = array("See no evil", "hear no evil");
      var threeItems = array("See no evil", "hear no evil", "speak no evil");

      Console.WriteLine(oneItem.Andify());
      Console.WriteLine(twoItems.Andify());
      Console.WriteLine(threeItems.Andify());
   }
}