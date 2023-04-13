using System;
using System.Threading;
using System.Threading.Tasks;
using Core.DataStructures;
using Core.Dates.DateIncrements;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Applications.Async.AsyncFunctions;
using static Core.Arrays.ArrayFunctions;
using static Core.Lambdas.LambdaFunctions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.Lazy.LazyRepeatingMonads;
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
               throw fail($"Interrupting at {result}");
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
      if (_value is (true, var value))
      {
         Console.WriteLine($"Value is {value}");
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
      var _text = maybe<string>() & 1 > 0 & "foobar";
      if (_text is (true, var text1))
      {
         Console.WriteLine(text1);
      }

      _text = func(() => 1 > 0).SomeIf(() => "foobar");
      if (_text is (true, var text2))
      {
         Console.WriteLine(text2);
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
      if (_result is (true, var result))
      {
         Console.WriteLine(result);
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
      var _result = result<string>() & date.Second < 30 & "seconds < 30" & fail(date.Second.Plural($"Only {"second(s)"}"));
      if (_result is (true, var result))
      {
         Console.WriteLine(result);
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
      if (_maybe is (true, var value))
      {
         Console.WriteLine(value);
      }

      var x = 1;
      var y = 10;
      y -= 10;
      var _result = tryTo(() => x / y);

      var message = _result is (true, var result) ? result.ToString() : _result.Exception.Message;
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
      if (_number is (true, var number))
      {
         Console.WriteLine(number);
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
      if (_result is (true, var result))
      {
         Console.WriteLine(result);
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

      if (_one is (true, var one))
      {
         Console.WriteLine(one);
      }

      if (_two is (true, var two))
      {
         Console.WriteLine(two);
      }

      if (_three is (true, var three))
      {
         Console.WriteLine(three);
      }
   }

   [TestMethod]
   public void LazyMaybe2Test()
   {
      var _one = lazy.maybe<string>();
      var _two = lazy.maybe<string>();
      var _three = lazy.maybe<string>();

      if (_one.ValueOf("one") is (true, var one))
      {
         Console.WriteLine(one);
      }
      else if (_two.ValueOf("two") is (true, var two))
      {
         Console.WriteLine(two);
      }
      else if (_three.ValueOf("three") is (true, var three))
      {
         Console.WriteLine(three);
      }
   }

   [TestMethod]
   public void LazyRepeatingMonadsTest()
   {
      var stack = new MaybeStack<string>();
      foreach (var item in array("a", "b", "c", "d", "e", "f"))
      {
         stack.Push(item);
      }

      var _item = lazyRepeating.maybe<string>();
      while (_item.ValueOf(stack.Pop()) is (true, var item))
      {
         Console.WriteLine(item);
      }
   }

   [TestMethod]
   public void ChainedLazyMonadsTest()
   {
      var _first = lazy.maybe<string>(() => "foobar");
      var _second = _first.Then(s => s.Length);
      if (_second)
      {
         Console.WriteLine($"Length: {_second} = 6");
      }
   }

   protected static Result<string> getResult(string text, bool success, int index) => success ? text : fail($"Failure {index}");

   [TestMethod]
   public void ChainedResultsTest()
   {
      var _first = lazy.result(() => getResult("alpha", true, 1));
      var _second = _first.Then(_ => getResult("bravo", false, 2));
      var _third = _second.Then(_ => getResult("charlie", true, 3));
      if (_third is (true, var third))
      {
         Console.WriteLine((string)_first);
         Console.WriteLine((string)_second);
         Console.WriteLine(third);
      }
      else
      {
         Console.WriteLine(_third.Exception);
      }
   }

   [TestMethod]
   public void ChainedResults2Test()
   {
      var _first = lazy.result(() => getResult("alpha", true, 1));
      var _second = _first.Then(_ => getResult("bravo", true, 2));
      var _third = _second.Then(_ => getResult("charlie", true, 3));
      if (_third)
      {
         Console.WriteLine((string)_first);
         Console.WriteLine((string)_second);
         Console.WriteLine((string)_third);
      }
      else
      {
         Console.WriteLine(_third.Exception);
      }
   }

   [TestMethod]
   public void ChainedResults3Test()
   {
      var _first = lazy.result(() => getResult("alpha", false, 1));
      var _second = _first.Then(_ => getResult("bravo", false, 2));
      var _third = _second.Then(_ => getResult("charlie", false, 3));
      if (_third)
      {
         Console.WriteLine((string)_first);
         Console.WriteLine((string)_second);
         Console.WriteLine((string)_third);
      }
      else
      {
         Console.WriteLine(_third.Exception);
      }
   }

   [TestMethod]
   public void MaybeFunctionTest()
   {
      var greet = true;
      var _value = maybe<string>() & greet & "world";
      if (_value is (true, var who))
      {
         Console.WriteLine($"hello {who}!");
      }
      else
      {
         Console.WriteLine("?");
      }
   }

   [TestMethod]
   public void LinkTest()
   {
      Maybe<int> _result1 = 30;
      Maybe<int> _result2 = 10;
      Maybe<int> _result3 = 2;

      var _sum =
         from r1 in _result1
         from r2 in _result2
         where r1 > 0
         select r1 - r2 into temp
         from r3 in _result3
         select temp * r3;
      if (_sum is (true, var sum))
      {
         Console.WriteLine(sum);
      }
      else
      {
         Console.WriteLine("?");
      }
   }

   [TestMethod]
   public void SequenceTest()
   {
      var _first = new[] { 0, 1, 2 }.FirstOrNone(x => x == 1);
      if (_first is (true, var first))
      {
         Console.WriteLine(first);
      }
   }

   protected class Request
   {
      public Request(string name, string email)
      {
         Name = name;
         Email = email;
      }

      public string Name { get; }

      public string Email { get; }

      public void SendMail() => Console.WriteLine($"sending to {Name} via {Email}");
   }

   [TestMethod]
   public void ResultTest()
   {
      Result<Request> validateInput(Request input)
      {
         if (input.Name.IsEmpty())
         {
            return fail("No must not be blank");
         }

         if (input.Email.IsEmpty())
         {
            return fail("Email must not be blank");
         }

         return input;
      }

      var request = new Request("Todd", "toddcoder@comcast.net");
      var _result = validateInput(request);
      if (_result is (true, var result))
      {
         result.SendMail();
      }
   }

   [TestMethod]
   public void EitherTest()
   {
      Either<Request, string> validateInput(Request input)
      {
         if (input.Name.IsEmpty())
         {
            return "No must not be blank";
         }

         if (input.Email.IsEmpty())
         {
            return "Email must not be blank";
         }

         return input;
      }

      var request = new Request("Todd", "toddcoder@comcast.net");
      var _result = validateInput(request);
      switch (_result)
      {
         case (true, var result, _):
            result.SendMail();
            break;
         case (false, _, var message):
            Console.WriteLine(message);
            break;
      }
   }

   [TestMethod]
   public void MaxedChainingTest()
   {
      Optional<string> _one = "one";
      Maybe<string> _two = "two";
      Result<string> _three = "three";

      var _all =
         from one in _one
         from two in _two
         from three in _three
         select one + two + three;
      if (_all is (true, var all))
      {
         Console.WriteLine(all);
      }
   }
}