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
   public class ResultAssertion : IAssertion<Result>
   {
      protected Result result;
      protected List<Constraint> constraints;
      protected bool not;
      protected string name;

      public ResultAssertion(Result result)
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

      public Result Value => result;

      public IEnumerable<Constraint> Constraints => constraints;

      public IAssertion<Result> Named(string name)
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

      public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, Result>(this, args);

      public Result Force() => force(this);

      public Result Force(string message) => force(this, message);

      public Result Force(Func<string> messageFunc) => force(this, messageFunc);

      public Result Force<TException>(params object[] args) where TException : Exception => force<TException, Result>(this, args);

      public TResult Force<TResult>() => throw "Can't convert Result to another type".Throws();

      public TResult Force<TResult>(string message) => throw message.Throws();

      public TResult Force<TResult>(Func<string> messageFunc) => throw messageFunc().Throws();

      public TResult Force<TException, TResult>(params object[] args) where TException : Exception => Force<TResult>();

      public IResult<Result> OrFailure() => orFailure(this);

      public IResult<Result> OrFailure(string message) => orFailure(this, message);

      public IResult<Result> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

      public Maybe<Result> OrNone() => orNone(this);

      public async Task<ICompletion<Result>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

      public async Task<ICompletion<Result>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

      public async Task<ICompletion<Result>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
      {
         return await orFailureAsync(this, messageFunc, token);
      }

      public bool OrReturn() => orReturn(this);
   }
}