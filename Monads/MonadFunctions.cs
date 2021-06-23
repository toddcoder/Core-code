using System;

namespace Core.Monads
{
   public static class MonadFunctions
   {
      public static Maybe<TParent> some<TChild, TParent>(TChild value) where TChild : TParent
      {
         return new Some<TParent>(value);
      }

      public static Maybe<TParent> someAs<TChild, TParent>(TChild value) where TChild : class, TParent where TParent : class
      {
         return new Some<TParent>(value);
      }

      public static Maybe<T> none<T>() => new None<T>();

      public static Result<T> success<T>(T value) => new Success<T>(value);

      public static Result<TParent> successAs<TChild, TParent>(TChild value) where TChild : class, TParent where TParent : class
      {
         return new Success<TParent>(value);
      }

      public static Result<T> failure<T>(Exception exception) => new Failure<T>(exception);

      public static Matched<TParent> matched<TChild, TParent>(TChild value) where TChild : TParent
      {
         return new Match<TParent>(value);
      }

      public static Matched<TParent> matchedAs<TChild, TParent>(TChild value) where TChild : class where TParent : class
      {
         return new Match<TParent>(value as TParent);
      }

      public static Matched<T> noMatch<T>() => new NoMatch<T>();

      public static Matched<T> failedMatch<T>(Exception exception) => new FailedMatch<T>(exception);

      public static Matched<T> isMatched<T>(bool test, Func<T> result)
      {
         try
         {
            return test ? result().Match() : noMatch<T>();
         }
         catch (Exception exception)
         {
            return failedMatch<T>(exception);
         }
      }

      public static Matched<T> isMatched<T>(bool test, Func<Matched<T>> result)
      {
         try
         {
            return test ? result() : noMatch<T>();
         }
         catch (Exception exception)
         {
            return failedMatch<T>(exception);
         }
      }

      public static Maybe<T> maybe<T>(bool test, Func<T> ifTrue) => test ? ifTrue().Some() : none<T>();

      public static Maybe<T> maybe<T>(bool test, Func<Maybe<T>> ifTrue) => test ? ifTrue() : none<T>();

      public static Completion<T> cancelled<T>() => new Cancelled<T>();

      public static Completion<T> interrupted<T>(Exception exception) => new Interrupted<T>(exception);

      public static Result<T> assert<T>(bool test, Func<T> ifTrue, Func<string> ifFalse)
      {
         try
         {
            return test ? ifTrue().Success() : ifFalse().Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static Result<T> assert<T>(bool test, Func<Result<T>> ifTrue, Func<string> ifFalse)
      {
         try
         {
            return test ? ifTrue() : ifFalse().Failure<T>();
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static Result<Unit> assert(bool test, Func<string> ifFalse)
      {
         try
         {
            return test ? Unit.Success() : ifFalse().Failure<Unit>();
         }
         catch (Exception exception)
         {
            return failure<Unit>(exception);
         }
      }
   }
}