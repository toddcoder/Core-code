using System;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Matching.MultiMatching
{
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

      public static MultiMatcher<T> operator &(MultiMatcher<T> multiMatcher, Func<T> func) => multiMatcher.Else(func);

      protected List<PatternAction> patternActions;
      protected Maybe<Func<T>> _defaultResult;

      internal MultiMatcher()
      {
         patternActions = new List<PatternAction>();
         _defaultResult = nil;
      }

      public Case When(Pattern pattern) => new(this, pattern);

      internal void AddPattern(Pattern pattern, Func<MatchResult, T> func) => patternActions.Add(new PatternAction(pattern, func));

      public MultiMatcher<T> Else(Func<T> func)
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
            if (input.Matches(pattern).Map(out var result))
            {
               try
               {
                  return func(result);
               }
               catch (Exception exception)
               {
                  return exception;
               }
            }
         }

         if (_defaultResult.Map(out var defaultAction))
         {
            try
            {
               return defaultAction();
            }
            catch (Exception exception)
            {
               return exception;
            }
         }

         return nil;
      }
   }
}