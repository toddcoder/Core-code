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
      protected string name;

      public MatcherAssertion(Matcher matcher)
      {
         this.matcher = matcher;
         constraints = new List<Constraint>();
         not = false;
         name = "Matcher";
      }

      public MatcherAssertion Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

      public Matcher Value => matcher;

      public IEnumerable<Constraint> Constraints => constraints;

      protected MatcherAssertion add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not, name, Value));
         not = false;

         return this;
      }

      public MatcherAssertion Match(string input, string pattern, bool ignoreCase, bool multiline)
      {
         return add(() => matcher.IsMatch(input, pattern, ignoreCase, multiline), $"$name must $not match \"{pattern}\"");
      }

      public MatcherAssertion Match(string input, string pattern, RegexOptions options)
      {
         return add(() => matcher.IsMatch(input, pattern, options), $"$name must $not match \"{pattern}\"");
      }

      public MatcherAssertion HaveMatchCountOf(int matchCount)
      {
         return add(() => matcher.MatchCount >= matchCount, $"$name must $not have a match count of at least {matchCount}");
      }

      public MatcherAssertion HaveGroupCountOf(int groupCount)
      {
         return HaveMatchCountOf(1)
            .add(() => matcher.GroupCount(0) >= groupCount, $"$name must $not have a group count of at least {groupCount}");
      }

      public IAssertion<Matcher> Named(string name)
      {
         this.name = name;
         return this;
      }

      public void OrThrow() => orThrow(this);

      public void OrThrow(string message) => orThrow(this, message);

      public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

      public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, Matcher>(this, args);

      public Matcher Force() => force(this);

      public Matcher Force(string message) => force(this, message);

      public Matcher Force(Func<string> messageFunc) => force(this, messageFunc);

      public Matcher Force<TException>(params object[] args) where TException : Exception => force<TException, Matcher>(this, args);

      public TResult Force<TResult>() => throw "Can't convert Matcher to another type".Throws();

      public TResult Force<TResult>(string message) => throw message.Throws();

      public TResult Force<TResult>(Func<string> messageFunc) => throw messageFunc().Throws();

      public TResult Force<TException, TResult>(params object[] args) where TException : Exception => Force<TResult>();

      public IResult<Matcher> OrFailure() => orFailure(this);

      public IResult<Matcher> OrFailure(string message) => orFailure(this, message);

      public IResult<Matcher> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public IMaybe<Matcher> OrNone() => orNone(this);

      public async Task<ICompletion<Matcher>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<Matcher>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<ICompletion<Matcher>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }

      public bool OrReturn() => orReturn(this);
   }
}