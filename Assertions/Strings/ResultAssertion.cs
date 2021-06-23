using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Exceptions;
using Core.Matching;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Strings
{
   public class ResultAssertion : IAssertion<MatchResult>
   {
      protected MatchResult result;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public ResultAssertion(MatchResult result)
      {
         this.result = result;

         constraints = new List<Constraint>();
         not = false;
         name = "Result";
      }

      public ResultAssertion Not
      {
         get
         {
            not = true;
            return this;
         }
      }

      public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

      public MatchResult Value => result;

      public IEnumerable<Constraint> Constraints => constraints;

      public IAssertion<MatchResult> Named(string name)
      {
         this.name = name;
         return this;
      }

      protected ResultAssertion add(Func<bool> constraintFunction, string message)
      {
         constraints.Add(new Constraint(constraintFunction, message, not, name, Value));
         not = false;

         return this;
      }

      public ResultAssertion HaveMatchCountOf(int matchCount)
      {
         return add(() => result.MatchCount >= matchCount, $"$name must $not have a match count of at least {matchCount}");
      }

      public ResultAssertion HaveGroupCountOf(int groupCount)
      {
         return HaveMatchCountOf(1)
            .add(() => result.GroupCount(0) >= groupCount, $"$name must $not have a group count of at least {groupCount}");
      }

      public void OrThrow() => orThrow(this);

      public void OrThrow(string message) => orThrow(this, message);

      public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

      public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, MatchResult>(this, args);

      public MatchResult Force() => force(this);

      public MatchResult Force(string message) => force(this, message);

      public MatchResult Force(Func<string> messageFunc) => force(this, messageFunc);

      public MatchResult Force<TException>(params object[] args) where TException : Exception => force<TException, MatchResult>(this, args);

      public TResult Force<TResult>() => throw "Can't convert Result to another type".Throws();

      public TResult Force<TResult>(string message) => throw message.Throws();

      public TResult Force<TResult>(Func<string> messageFunc) => throw messageFunc().Throws();

      public TResult Force<TException, TResult>(params object[] args) where TException : Exception => Force<TResult>();

      public Result<MatchResult> OrFailure() => orFailure(this);

      public Result<MatchResult> OrFailure(string message) => orFailure(this, message);

      public Result<MatchResult> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public Maybe<MatchResult> OrNone() => orNone(this);

      public async Task<ICompletion<MatchResult>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<MatchResult>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<ICompletion<MatchResult>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }

      public bool OrReturn() => orReturn(this);
   }
}