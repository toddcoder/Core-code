using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Dates.DateIncrements;
using Core.Exceptions;
using Core.Monads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Monads.AttemptFunctions;

namespace Core.Tests
{
   internal class Counter
   {
      long maxValue;
      bool interrupt;

      public Counter(long maxValue, bool interrupt = false)
      {
         this.maxValue = maxValue;
         this.interrupt = interrupt;
      }

      public async Task<ICompletion<long>> CountAsync(CancellationTokenSource source)
      {
         return await runAsync(t =>
         {
            long result = 0;

            for (long i = 0; i < maxValue; i++)
            {
               result = i;
               if (interrupt && i > 0 && i % 1000 == 0)
               {
                  throw $"Interrupting at {result}".Throws();
               }
            }

            return result.Completed();
         }, source);
      }
   }

   [TestClass]
   public class MonadTests
   {
      [TestMethod]
      public void CompletedTest()
      {
         var counter = new Counter(100_000L);
         var source = new CancellationTokenSource(30.Seconds());
         var completion = Task.Run(() => counter.CountAsync(source), source.Token);
         if (completion.Result.If(out var value, out var anyException))
         {
            Console.WriteLine($"Value is {value}");
         }
         else if (anyException.If(out var exception))
         {
            Console.WriteLine($"Interrupted with: {exception.Message}");
         }
         else
         {
            Console.WriteLine("Cancelled");
         }
      }

      [TestMethod]
      public void CancelledTest()
      {
         var counter = new Counter(100_000L);
         var source = new CancellationTokenSource(30.Seconds());
         var completion = Task.Run(() => counter.CountAsync(source), source.Token);
         source.Cancel();
         if (completion.Result.If(out var value, out var anyException))
         {
            Console.WriteLine($"Value is {value}");
         }
         else if (anyException.If(out var exception))
         {
            Console.WriteLine($"Interrupted with: {exception.Message}");
         }
         else
         {
            Console.WriteLine("Cancelled");
         }
      }

      [TestMethod]
      public void InterruptedTest()
      {
         var counter = new Counter(100_000L, true);
         var source = new CancellationTokenSource(30.Seconds());
         var completion = Task.Run(() => counter.CountAsync(source), source.Token);
         if (completion.Result.If(out var value, out var anyException))
         {
            Console.WriteLine($"Value is {value}");
         }
         else if (anyException.If(out var exception))
         {
            Console.WriteLine($"Interrupted with: {exception.Message}");
         }
         else
         {
            Console.WriteLine("Cancelled");
         }
      }

      static async Task<ICompletion<int>> getOne(CancellationToken token) => await runAsync(t => 1.Completed(), token);

      static async Task<ICompletion<int>> getTwo(CancellationToken token) => await runAsync(t => 2.Completed(), token);

      static async Task<ICompletion<int>> getThree(CancellationToken token) => await runAsync(t => 3.Completed(), token);

      [TestMethod]
      public void RunAsyncTest()
      {
         var source = new CancellationTokenSource(30.Seconds());
         var token = source.Token;
         var result =
            from one in getOne(token)
            from two in getTwo(token)
            from three in getThree(token)
            select one + two + three;
         if (result.Result.If(out var six, out var anyException))
         {
            Console.WriteLine($"Value: {six}");
         }
         else if (anyException.If(out var exception))
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
         else
         {
            Console.WriteLine("Cancelled");
         }
      }
   }
}
