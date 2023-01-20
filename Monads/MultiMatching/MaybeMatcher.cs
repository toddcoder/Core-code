using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.MultiMatching;

public class MaybeMatcher<T, TResult>
{
   public class MaybeFunction
   {
      public MaybeFunction(Maybe<T> maybe, Func<T, TResult> func)
      {
         Maybe = maybe;
         Func = func;
      }

      public Maybe<T> Maybe { get; }

      public Func<T, TResult> Func { get; }

      public void Deconstruct(out Maybe<T> maybe, out Func<T, TResult> func)
      {
         maybe = Maybe;
         func = Func;
      }
   }

   public class Case
   {
      public static MaybeMatcher<T, TResult> operator &(Case @case, Func<T, TResult> func) => @case.Then(func);

      protected MaybeMatcher<T, TResult> maybeMatcher;

      public Case(MaybeMatcher<T, TResult> maybeMatcher, Maybe<T> maybe)
      {
         this.maybeMatcher = maybeMatcher;

         Maybe = maybe;
      }

      public Maybe<T> Maybe { get; }

      public MaybeMatcher<T, TResult> Then(Func<T, TResult> func)
      {
         maybeMatcher.AddMaybe(Maybe, func);
         return maybeMatcher;
      }
   }

   public static Case operator &(MaybeMatcher<T, TResult> maybeMatcher, Maybe<T> maybe) => maybeMatcher.When(maybe);

   public static MaybeMatcher<T, TResult> operator &(MaybeMatcher<T, TResult> maybeMatcher, Func<TResult> func) => maybeMatcher.Else(func);

   protected List<MaybeFunction> maybeFunctions;
   protected Maybe<Func<TResult>> _defaultFunction;

   internal MaybeMatcher()
   {
      maybeFunctions = new List<MaybeFunction>();
      _defaultFunction = nil;
   }

   public Case When(Maybe<T> maybe) => new(this, maybe);

   internal void AddMaybe(Maybe<T> maybe, Func<T, TResult> func) => maybeFunctions.Add(new MaybeFunction(maybe, func));

   public MaybeMatcher<T, TResult> Else(Func<TResult> func)
   {
      if (!_defaultFunction)
      {
         _defaultFunction = func;
      }

      return this;
   }

   public Maybe<TResult> Matches()
   {
      foreach (var (_maybe, func) in maybeFunctions)
      {
         if (_maybe)
         {
            return func(_maybe);
         }
      }

      if (_defaultFunction)
      {
         return _defaultFunction.Value();
      }
      else
      {
         return nil;
      }
   }
}

public class MaybeMatcher<T>
{
   public class MaybeAction
   {
      public MaybeAction(Maybe<T> maybe, Action<T> action)
      {
         Maybe = maybe;
         Action = action;
      }

      public Maybe<T> Maybe { get; }

      public Action<T> Action { get; }

      public void Deconstruct(out Maybe<T> maybe, out Action<T> action)
      {
         maybe = Maybe;
         action = Action;
      }
   }

   public class Case
   {
      public static MaybeMatcher<T> operator &(Case @case, Action<T> action) => @case.Then(action);

      protected MaybeMatcher<T> maybeMatcher;

      public Case(MaybeMatcher<T> maybeMatcher, Maybe<T> maybe)
      {
         this.maybeMatcher = maybeMatcher;

         Maybe = maybe;
      }

      public Maybe<T> Maybe { get; }

      public MaybeMatcher<T> Then(Action<T> action)
      {
         maybeMatcher.AddMaybe(Maybe, action);
         return maybeMatcher;
      }
   }

   public static Case operator &(MaybeMatcher<T> maybeMatcher, Maybe<T> maybe) => maybeMatcher.When(maybe);

   public static MaybeMatcher<T> operator &(MaybeMatcher<T> maybeMatcher, Action action) => maybeMatcher.Else(action);

   protected List<MaybeAction> maybeActions;
   protected Maybe<Action> _defaultAction;

   internal MaybeMatcher()
   {
      maybeActions = new List<MaybeAction>();
      _defaultAction = nil;
   }

   public Case When(Maybe<T> maybe) => new(this, maybe);

   internal void AddMaybe(Maybe<T> maybe, Action<T> action) => maybeActions.Add(new MaybeAction(maybe, action));

   public MaybeMatcher<T> Else(Action action)
   {
      if (!_defaultAction)
      {
         _defaultAction = action;
      }

      return this;
   }

   public Maybe<T> Matches()
   {
      foreach (var (_maybe, action) in maybeActions)
      {
         if (_maybe)
         {
            action(_maybe);
            return _maybe;
         }
      }

      if (_defaultAction)
      {
         _defaultAction.Value();
      }

      return nil;
   }
}