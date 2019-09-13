using System;
using System.Threading.Tasks;
using Core.Applications.AsyncEvents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class AsyncEventTests
   {
      event AsyncEventHandler<EventArgs> Greet;

      [TestMethod]
      public async Task AsyncEventTest()
      {
         Greet += (sender, e) => Task.Run(() => Console.WriteLine("Alpha"));
         Greet += (sender, e) => Task.Run(() => Console.WriteLine("Bravo"));
         Greet += (sender, e) => Task.Run(() => Console.WriteLine("Charlie"));

         await Greet.InvokeAsync(this, EventArgs.Empty);
      }
   }
}