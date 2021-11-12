using System;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Matching.MultiMatching
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

   public class MultiMatcher
   {
      protected string input;
      protected List<PatternAction> patternActions;
      protected Maybe<Action> _defaultAction;

      internal MultiMatcher(string input)
      {
         this.input = input;

         patternActions = new List<PatternAction>();
         _defaultAction = nil;
      }

      public Case Case(Pattern pattern) => new(this, pattern);

      internal void AddPattern(Pattern pattern, Action<MatchResult> action) => patternActions.Add(new PatternAction(pattern, action));

      public MultiMatcher Else(Action action)
      {
         if (_defaultAction.IsNone)
         {
            _defaultAction = action;
         }

         return this;
      }

      public Responding<MatchResult> Result()
      {
         foreach (var (pattern, action) in patternActions)
         {
            if (input.Matches(pattern).If(out var result))
            {
               try
               {
                  action(result);
                  return result;
               }
               catch (Exception exception)
               {
                  return exception;
               }
            }
         }

         if (_defaultAction.If(out var defaultAction))
         {
            try
            {
               defaultAction();
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