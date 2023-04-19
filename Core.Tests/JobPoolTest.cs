using System;
using Core.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class JobPoolTest
{
   [TestMethod]
   public void BasicTest()
   {
      var jobPool = new JobPool();
      for (var i = 0; i < 10; i++)
      {
         jobPool.Enqueue(display);
      }
      jobPool.Dispatch();
   }

   protected void display(int affinity)
   {
      for (var i = 0; i < 10; i++)
      {
         Console.WriteLine($"{affinity}: {i}");
      }
   }
}