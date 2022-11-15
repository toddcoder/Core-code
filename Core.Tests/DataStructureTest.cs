using System;
using Core.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class DataStructureTest
{
   [TestMethod]
   public void StackEnumerableTest()
   {
      var stack = new MaybeStack<int>();
      for (var i = 0; i < 10; i++)
      {
         stack.Push(i);
      }

      foreach (var item in stack.Popping())
      {
         Console.WriteLine(item);
      }

      Console.WriteLine("---");

      for (var i = 0; i < 10; i++)
      {
         stack.Push(i);
      }

      foreach (var item in stack)
      {
         Console.WriteLine(item);
      }
   }

   [TestMethod]
   public void StackWhileTest()
   {
      var stack = new MaybeStack<int>();
      for (var i = 0; i < 10; i++)
      {
         stack.Push(i);
      }

      while (stack.More())
      {
         if (stack.Last)
         {
            Console.WriteLine(~stack.Last);
         }
      }
   }
}