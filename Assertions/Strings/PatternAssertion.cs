﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Matching;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Assertions.Strings;

public class PatternAssertion : IAssertion<Pattern>
{
   protected Pattern pattern;
   protected List<Constraint> constraints;
   protected bool not;
   protected string name;

   public PatternAssertion(Pattern pattern)
   {
      this.pattern = pattern;

      constraints = new List<Constraint>();
      not = false;
      name = "Pattern";
   }

   public PatternAssertion Not
   {
      get
      {
         not = true;
         return this;
      }
   }

   public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

   public Pattern Value => pattern;

   public IEnumerable<Constraint> Constraints => constraints;

   public IAssertion<Pattern> Named(string name)
   {
      this.name = name;
      return this;
   }

   protected PatternAssertion add(Func<bool> constraintFunction, string message)
   {
      constraints.Add(new Constraint(constraintFunction, message, not, name, Value));
      not = false;

      return this;
   }

   public PatternAssertion MatchedBy(string input)
   {
      return add(() => input.Matches(pattern), $"$name must $not be matched by \"{pattern}\"");
   }

   public void OrThrow() => orThrow(this);

   public void OrThrow(string message) => orThrow(this, message);

   public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

   public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, Pattern>(this, args);

   public Pattern Force() => force(this);

   public Pattern Force(string message) => force(this, message);

   public Pattern Force(Func<string> messageFunc) => force(this, messageFunc);

   public Pattern Force<TException>(params object[] args) where TException : Exception => force<TException, Pattern>(this, args);

   public TResult Force<TResult>() => throw fail("Can't convert Pattern to another type");

   public TResult Force<TResult>(string message) => throw fail(message);

   public TResult Force<TResult>(Func<string> messageFunc) => throw fail(messageFunc());

   public TResult Force<TException, TResult>(params object[] args) where TException : Exception => Force<TResult>();

   public Optional<Pattern> OrFailure() => orFailure(this);

   public Optional<Pattern> OrFailure(string message) => orFailure(this, message);

   public Optional<Pattern> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

   public Optional<Pattern> OrNone() => orNone(this);

   public async Task<Completion<Pattern>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

   public async Task<Completion<Pattern>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

   public async Task<Completion<Pattern>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
   {
      return await orFailureAsync(this, messageFunc, token);
   }

   public bool OrReturn() => orReturn(this);
}