using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Applications.Async;
using Core.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Applications.Async.AsyncFunctions;

namespace Core.Tests
{
   [TestClass]
   public class AsyncTests
   {
      protected event AsyncEventHandler<EventArgs> Greet;

      [TestMethod]
      public async Task AsyncEventTest()
      {
         Greet += (_, _) => Task.Run(() => Console.WriteLine("Alpha"));
         Greet += (_, _) => Task.Run(() => Console.WriteLine("Bravo"));
         Greet += (_, _) => Task.Run(() => Console.WriteLine("Charlie"));

         await Greet.InvokeAsync(this, EventArgs.Empty);
      }

      [TestMethod]
      public async Task AsyncLockTest()
      {
         Console.WriteLine("Locking");
         using var source = new CancellationTokenSource();
         using (await asyncLock(source.Token))
         {
            Console.WriteLine("Unlocked");
            await Task.Delay(1000);
            Console.WriteLine("Done");
         }
      }

      protected static void waitRandom(int affinity)
      {
         var random = new Random();
         var wait = random.Next(500, 20000);
         Thread.Sleep(wait);

         Console.WriteLine($"{affinity} - waited {wait}");
      }

      [TestMethod]
      public void JobPoolTest()
      {
         var jobPool = new JobPool();
         for (var i = 0; i < 100; i++)
         {
            jobPool.Enqueue(waitRandom);
         }

         jobPool.Dispatch();
      }
   }
}