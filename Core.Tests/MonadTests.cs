using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Dates.DateIncrements;
using Core.Exceptions;
using Core.Monads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Applications.Async.AsyncFunctions;
using static Core.Lambdas.LambdaFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Tests
{
   internal class Counter
   {
      protected long maxValue;
      protected bool interrupt;

      public Counter(long maxValue, bool interrupt = false)
      {
         this.maxValue = maxValue;
         this.interrupt = interrupt;
      }

      public async Task<Completion<long>> CountAsync(CancellationTokenSource source)
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

      protected static async Task<Completion<int>> getOne(CancellationToken token) => await runAsync(t => 1.Completed(), token);

      protected static async Task<Completion<int>> getTwo(CancellationToken token) => await runAsync(t => 2.Completed(), token);

      protected static async Task<Completion<int>> getThree(CancellationToken token) => await runAsync(t => 3.Completed(), token);

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

      [TestMethod]
      public void ExtensionsTest()
      {
         var result = (1, "foo").Success();
         if (result.ValueOrCast<int, string, Unit>(out var i, out var s, out var asUnit))
         {
            Console.WriteLine(i);
            Console.WriteLine(s);
         }
         else
         {
            Console.WriteLine(asUnit);
         }
      }

      [TestMethod]
      public void MappingExtensionsTest()
      {
         var result = (1, "foobar").Some();
         var result1 = result.Map((i, s) => i + s);
         if (result1.If(out var aString))
         {
            Console.WriteLine(aString);
         }

         var result2 = result.Map((i, s) => (s, i));
         if (result2.If(out aString, out var anInt))
         {
            Console.WriteLine(aString);
            Console.WriteLine(anInt);
         }
      }

      [TestMethod]
      public void SomeIfTest()
      {
         if ((1 > 0).SomeIf(() => "foobar").If(out var text))
         {
            Console.WriteLine(text);
         }

         if (func(() => 1 > 0).SomeIf(() => "foobar").If(out text))
         {
            Console.WriteLine(text);
         }
      }

      [TestMethod]
      public void MaybeOrTest()
      {
         var some1 = 1.Some();
         var some2 = 2.Some();
         var none = none<int>();

         var or1 = some1 | none;
         var or2 = none | some2;
         var or3 = none | none;
         var or4 = some1 | some2;
         var or5 = some2 | some1;

         Console.WriteLine($"some1 | none  = {or1}");
         Console.WriteLine($"none  | some2 = {or2}");
         Console.WriteLine($"none  | none  = {or3}");
         Console.WriteLine($"some1 | some2 = {or4}");
         Console.WriteLine($"some2 | some1 = {or5}");
      }

      [TestMethod]
      public void ResultOrTest()
      {
         var success1 = 1.Success();
         var success2 = 2.Success();
         var failure = "Divide by zero".Failure<int>();

         var or1 = success1 | failure;
         var or2 = failure | success2;
         var or3 = failure | failure;
         var or4 = success1 | success2;
         var or5 = success2 | success1;

         Console.WriteLine($"success1 | failure  = {or1}");
         Console.WriteLine($"failure  | success2 = {or2}");
         Console.WriteLine($"failure  | failure  = {or3}");
         Console.WriteLine($"success1 | success2 = {or4}");
         Console.WriteLine($"success2 | success1 = {or5}");
      }
   }
}