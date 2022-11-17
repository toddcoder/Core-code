using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Dates.DateIncrements;
using Core.Exceptions;
using Core.Monads;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Applications.Async.AsyncFunctions;
using static Core.Lambdas.LambdaFunctions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.Lazy.LazyMonadFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Monads.MultiMatching.MonadMatcherFunctions;

namespace Core.Tests;

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
      return await runAsync(_ =>
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
      var _value = completion.Result;
      if (_value)
      {
         Console.WriteLine($"Value is {~_value}");
      }
      else if (_value.AnyException)
      {
         Console.WriteLine($"Interrupted with: {_value.Exception.Message}");
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
      var _value = completion.Result;
      if (_value)
      {
         Console.WriteLine($"Value is {_value}");
      }
      else if (_value.AnyException)
      {
         Console.WriteLine($"Interrupted with: {_value.Exception.Message}");
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
      var _value = completion.Result;
      if (_value)
      {
         Console.WriteLine($"Value is {_value}");
      }
      else if (_value.AnyException)
      {
         Console.WriteLine($"Interrupted with: {_value.Exception.Message}");
      }
      else
      {
         Console.WriteLine("Cancelled");
      }
   }

   protected static async Task<Completion<int>> getOne(CancellationToken token) => await runAsync(_ => 1.Completed(), token);

   protected static async Task<Completion<int>> getTwo(CancellationToken token) => await runAsync(_ => 2.Completed(), token);

   protected static async Task<Completion<int>> getThree(CancellationToken token) => await runAsync(_ => 3.Completed(), token);

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
      var _six = result.Result;
      if (_six)
      {
         Console.WriteLine($"Value: {_six}");
      }
      else if (_six.AnyException)
      {
         Console.WriteLine($"Exception: {_six.Exception.Message}");
      }
      else
      {
         Console.WriteLine("Cancelled");
      }
   }

   [TestMethod]
   public void MappingExtensionsTest()
   {
      Maybe<(int, string)> _result = (1, "foobar");
      var _result1 = _result.Map((i, s) => i + s);
      if (_result1)
      {
         string value = _result1;
         Console.WriteLine(value);
      }

      var _result2 = _result.Map((i, s) => (s, i));
      if (_result2)
      {
         var (aString, anInt) = ((string, int))_result2;
         Console.WriteLine(aString);
         Console.WriteLine(anInt);
      }
   }

   [TestMethod]
   public void SomeIfTest()
   {
      var _text = (1 > 0).SomeIf(() => "foobar");
      if (_text)
      {
         Console.WriteLine(~_text);
      }

      _text = func(() => 1 > 0).SomeIf(() => "foobar");
      if (_text)
      {
         Console.WriteLine(~_text);
      }
   }

   [TestMethod]
   public void MaybeOrTest()
   {
      Maybe<int> some1 = 1;
      Maybe<int> some2 = 2;
      Maybe<int> none = nil;

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
      Result<int> success1 = 1;
      Result<int> success2 = 2;
      Result<int> failure = fail("Divide by zero");

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

   [TestMethod]
   public void ImplicitMaybeTest()
   {
      Maybe<string> maybe = "foobar";
      Console.WriteLine(maybe.ToString());

      maybe = nil;
      Console.WriteLine(maybe.ToString());
   }

   [TestMethod]
   public void ImplicitResultTest()
   {
      Result<string> result = "Good!";
      Console.WriteLine(result.ToString());

      result = fail("Bad!");
      Console.WriteLine(result.ToString());
   }

   [TestMethod]
   public void NullTupleItemTest()
   {
      (int, string, int) items = (1, null, 10);
      Maybe<(int, string, int)> _items = items;
      Console.WriteLine(!_items);
   }

   [TestMethod]
   public void BooleanTest()
   {
      Maybe<string> _string = nil;
      if (!_string)
      {
         Console.WriteLine("not");
      }

      _string = "some";
      if (_string)
      {
         Console.WriteLine("is");
      }
   }

   [TestMethod]
   public void ImplicitCastToParameterTest()
   {
      string defaultTo()
      {
         Console.WriteLine("defaultTo called");
         return "default";
      }

      var _maybe = (Maybe<string>)"Test";
      var text = _maybe | "nothing";
      Console.WriteLine(text);

      text = _maybe | "def" + "ault";
      Console.WriteLine(text);

      _maybe = nil;
      text = _maybe | "nothing";
      Console.WriteLine(text);

      text = _maybe | defaultTo;
      Console.WriteLine(text);

      var _result = (Result<int>)153;
      var result = _result | -1;
      Console.WriteLine(result);

      _result = fail("No number found");
      result = _result | -1;
      Console.WriteLine(result);

      var exception = _result.Exception;
      Console.WriteLine(exception.Message);
   }

   [TestMethod]
   public void MaybeIfTest()
   {
      var date = DateTime.Now;
      var _result = maybe<string>() & date.Second < 30 & "seconds < 30";
      if (_result)
      {
         Console.WriteLine(~_result);
      }
      else
      {
         Console.WriteLine(date.Second);
      }
   }

   [TestMethod]
   public void MaybeIfWithDefaultTest()
   {
      var date = DateTime.Now;
      var result = maybe<string>() & date.Second < 30 & "seconds < 30" | date.Second.ToString;
      Console.WriteLine(result);
   }

   [TestMethod]
   public void ResultIfTest()
   {
      var date = DateTime.Now;
      var _result = result<string>() & date.Second < 30 & "seconds < 30" & fail($"Only {"second(s)".Plural(date.Second)}");
      if (_result)
      {
         Console.WriteLine(~_result);
      }
      else
      {
         Console.WriteLine(_result.Exception.Message);
      }
   }

   [TestMethod]
   public void IfTest()
   {
      Maybe<string> _maybe = "foobar";
      if (_maybe)
      {
         Console.WriteLine(~_maybe);
      }

      var x = 1;
      var y = 10;
      y -= 10;
      var _result = tryTo(() => x / y);

      var message = _result ? (~_result).ToString() : _result.Exception.Message;
      Console.WriteLine(message);
   }

   [TestMethod]
   public void DefaultShortCircuitTest()
   {
      int defaultValue()
      {
         Console.WriteLine("Calling default");
         return 153;
      }

      Maybe<int> _number = 1;
      Console.WriteLine(_number | defaultValue);
      Console.WriteLine(_number ? _number : () => defaultValue());
   }

   [TestMethod]
   public void MaybeIfTest2()
   {
      var text = "Text";
      var _text = text.MaybeIf(t => t.IsNotEmpty());
      if (_text)
      {
         Console.WriteLine($"{_text} is not empty");
      }
      else
      {
         Console.WriteLine("empty");
      }
   }

   [TestMethod]
   public void MaybeValueOperator()
   {
      Maybe<int> _number = 153;
      if (_number)
      {
         Console.WriteLine(~_number);
      }
   }

   [TestMethod]
   public void MonadMatcherTest()
   {
      Maybe<int> _one = nil;
      Maybe<int> _two = 2;
      Maybe<int> _three = nil;

      var matcher = maybeMatcher<int, string>()
         & _one & (_ => "one")
         & _two & (_ => "two")
         & _three & (_ => "three")
         & (() => "other");

      var _result = matcher.Matches();
      if (_result)
      {
         Console.WriteLine(~_result);
      }
      else
      {
         Console.WriteLine("nil");
      }

      var matcher2 = maybeMatcher<int>()
         & _one & (o => Console.WriteLine($"one is {o}"))
         & _two & (o => Console.WriteLine($"two is {o}"))
         & _three & (o => Console.WriteLine($"three is {o}"))
         & (() => Console.WriteLine("None"));

      matcher2.Matches();
   }

   [TestMethod]
   public void LazyMaybeTest()
   {
      var _one = lazy.maybe<string>(() =>
      {
         Console.WriteLine("Ensured _one");
         return "one";
      });

      var _two = lazy.maybe<string>(() =>
      {
         Console.WriteLine("Ensured _two");
         return nil;
      });

      var _three = lazy.maybe<string>(() =>
      {
         Console.WriteLine("Ensured _three");
         return "three";
      });

      if (_one)
      {
         Console.WriteLine(~_one);
      }

      if (_two)
      {
         Console.WriteLine(~_two);
      }

      if (_three)
      {
         Console.WriteLine(~_three);
      }
   }

   [TestMethod]
   public void LazyMaybe2Test()
   {
      var _one = lazy.maybe<string>();
      var _two = lazy.maybe<string>();
      var _three = lazy.maybe<string>();

      if (_one.ValueOf("one"))
      {
         Console.WriteLine(~_one);
      }
      else if (_two.ValueOf("two"))
      {
         Console.WriteLine(~_two);
      }
      else if (_three.ValueOf("three"))
      {
         Console.WriteLine(~_three);
      }
   }
}