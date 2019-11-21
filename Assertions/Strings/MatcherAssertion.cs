using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Exceptions;
using Core.Monads;
using Core.RegularExpressions;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Strings
{
   public class MatcherAssertion : IAssertion<Matcher>
   {
      protected Matcher matcher;
      protected List<Constraint> constraints;
      protected bool not;
      public MatcherAssertion(Matcher matcher)
      {
         this.matcher = matcher;
         constraints = new List<Constraint>();
         not = false;
      }

      public MatcherAssertion Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      public bool BeTrue() => beTrue(this);

      public Matcher Value => matcher;

      public IEnumerable<Constraint> Constraints => constraints;

      protected MatcherAssertion add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not));
         not = false;

         return this;
      }

      public MatcherAssertion Match(string input, string pattern, bool ignoreCase, bool multiline)
      {
         return add(() => matcher.IsMatch(input, pattern, ignoreCase, multiline), $"\"{input}\" must $not match \"{pattern}\"");
      }

      public MatcherAssertion Match(string input, string pattern, RegexOptions options)
      {
         return add(() => matcher.IsMatch(input, pattern, options), $"\"{input}\" must $not match \"{pattern}\"");
      }

      public MatcherAssertion HaveMatchCountOf(int matchCount)
      {
         return add(() => matcher.MatchCount >= matchCount, $"Matcher must $not have a match count of at least {matchCount}");
      }

      public MatcherAssertion HaveGroupCountOf(int groupCount)
      {
         return HaveMatchCountOf(1)
            .add(() => matcher.GroupCount(0) >= groupCount, $"Matcher must $not have a group count of at least {groupCount}");
      }

      public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, Matcher>(this, args);

      public Matcher Ensure() => ensure(this);

      public Matcher Ensure(string message) => ensure(this, message);

      public Matcher Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public Matcher Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, Matcher>(this, args);

      public TResult Ensure<TResult>() => throw "Can't convert Matcher to another type".Throws();

      public TResult Ensure<TResult>(string message) => throw message.Throws();

      public TResult Ensure<TResult>(Func<string> messageFunc) => throw messageFunc().Throws();

      public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception => Ensure<TResult>();

      public IResult<Matcher> Try() => @try(this);

      public IResult<Matcher> Try(string message) => @try(this, message);

      public IResult<Matcher> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public IMaybe<Matcher> Maybe() => maybe(this);

      public async Task<ICompletion<Matcher>> TryAsync(CancellationToken token) => await tryAsync(this, token);

      public async Task<ICompletion<Matcher>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<Matcher>> TryAsync(Func<string> messageFunc, CancellationToken token) => await tryAsync(this, messageFunc, token);
   }
}