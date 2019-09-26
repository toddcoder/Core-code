using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Applications.Async;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Applications.Async.AsyncFunctions;

namespace Core.Tests
{
   [TestClass]
   public class AsyncTests
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

      [TestMethod]
      public async Task AsyncLockTest()
      {
         Console.WriteLine("Locking");
         using (var source = new CancellationTokenSource())
         using (await asyncLock(source.Token))
         {
            Console.WriteLine("Unlocked");
            await Task.Delay(1000);
            Console.WriteLine("Done");
         }
      }
   }
}