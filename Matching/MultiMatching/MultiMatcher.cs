using System;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Matching.MultiMatching;

public class MultiMatcher<T>
{
   public class PatternAction
   {
      public PatternAction(Pattern pattern, Func<MatchResult, T> func)
      {
         Pattern = pattern;
         Func = func;
      }

      public Pattern Pattern { get; }

      public Func<MatchResult, T> Func { get; }

      public void Deconstruct(out Pattern pattern, out Func<MatchResult, T> func)
      {
         pattern = Pattern;
         func = Func;
      }
   }

   public class Case
   {
      public static MultiMatcher<T> operator &(Case @case, Func<MatchResult, T> func) => @case.Then(func);

      protected MultiMatcher<T> multiMatcher;

      public Case(MultiMatcher<T> multiMatcher, Pattern pattern)
      {
         this.multiMatcher = multiMatcher;
         Pattern = pattern;
      }

      public Pattern Pattern { get; }

      public MultiMatcher<T> Then(Func<MatchResult, T> func)
      {
         multiMatcher.AddPattern(Pattern, func);
         return multiMatcher;
      }
   }

   public static Case operator &(MultiMatcher<T> multiMatcher, Pattern pattern) => multiMatcher.When(pattern);

   public static MultiMatcher<T> operator &(MultiMatcher<T> multiMatcher, Func<string, T> func) => multiMatcher.Else(func);

   protected List<PatternAction> patternActions;
   protected Maybe<Func<string, T>> _defaultResult;

   internal MultiMatcher()
   {
      patternActions = new List<PatternAction>();
      _defaultResult = nil;
   }

   public Case When(Pattern pattern) => new(this, pattern);

   internal void AddPattern(Pattern pattern, Func<MatchResult, T> func) => patternActions.Add(new PatternAction(pattern, func));

   public MultiMatcher<T> Else(Func<string, T> func)
   {
      if (!_defaultResult)
      {
         _defaultResult = func;
      }

      return this;
   }

   public Responding<T> Matches(string input)
   {
      foreach (var (pattern, func) in patternActions)
      {
         var _result = input.Matches(pattern);
         if (_result)
         {
            try
            {
               return func(_result);
            }
            catch (Exception exception)
            {
               return exception;
            }
         }
      }

      if (_defaultResult)
      {
         try
         {
            return _defaultResult.Value(input);
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      return nil;
   }
}

public class MultiMatcher
{
   public class PatternAction
   {
      public PatternAction(Pattern pattern, Action<MatchResult> action)
      {
         Pattern = pattern;
         Action = action;
      }

      public Pattern Pattern { get; }

      public Action<MatchResult> Action { get; }

      public void Deconstruct(out Pattern pattern, out Action<MatchResult> action)
      {
         pattern = Pattern;
         action = Action;
      }
   }

   public class Case
   {
      public static MultiMatcher operator &(Case @case, Action<MatchResult> func) => @case.Then(func);

      protected MultiMatcher multiMatcher;

      public Case(MultiMatcher multiMatcher, Pattern pattern)
      {
         this.multiMatcher = multiMatcher;
         Pattern = pattern;
      }

      public Pattern Pattern { get; }

      public MultiMatcher Then(Action<MatchResult> action)
      {
         multiMatcher.AddPattern(Pattern, action);
         return multiMatcher;
      }
   }

   public static Case operator &(MultiMatcher multiMatcher, Pattern pattern) => multiMatcher.When(pattern);

   public static MultiMatcher operator &(MultiMatcher multiMatcher, Action<string> action) => multiMatcher.Else(action);

   protected List<PatternAction> patternActions;
   protected Maybe<Action<string>> _defaultAction;

   internal MultiMatcher()
   {
      patternActions = new List<PatternAction>();
      _defaultAction = nil;
   }

   public Case When(Pattern pattern) => new(this, pattern);

   internal void AddPattern(Pattern pattern, Action<MatchResult> action) => patternActions.Add(new PatternAction(pattern, action));

   public MultiMatcher Else(Action<string> action)
   {
      if (!_defaultAction)
      {
         _defaultAction = action;
      }

      return this;
   }

   public Responding<Unit> Matches(string input)
   {
      foreach (var (pattern, action) in patternActions)
      {
         var _result = input.Matches(pattern);
         if (_result)
         {
            try
            {
               action(_result);
               return unit;
            }
            catch (Exception exception)
            {
               return exception;
            }
         }
      }

      if (_defaultAction)
      {
         try
         {
            _defaultAction.Value(input);
            return unit;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      return nil;
   }
}