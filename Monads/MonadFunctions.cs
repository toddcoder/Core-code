using System;

namespace Core.Monads;

public static class MonadFunctions
{
   public static readonly Nil nil = new();

   public static readonly Unit unit = new();

   public static Exception fail(string message) => new ApplicationException(message);

   public static Optional<TParent> some<TChild, TParent>(TChild value) where TChild : TParent
   {
      return new Some<TParent>(value);
   }

   public static Optional<TParent> someAs<TChild, TParent>(TChild value) where TChild : class, TParent where TParent : class
   {
      return new Some<TParent>(value);
   }

   public static Optional<T> success<T>(T value) => new Success<T>(value);

   public static Optional<TParent> successAs<TChild, TParent>(TChild value) where TChild : class, TParent where TParent : class
   {
      return new Success<TParent>(value);
   }

   [Obsolete("Use exception")]
   public static Optional<T> failure<T>(Exception exception) => new Failure<T>(exception);

   [Obsolete("Use exception")]
   public static Optional<T> failedResponse<T>(Exception exception) => new Failed<T>(exception);

   [Obsolete]
   public static Matched<TParent> matched<TChild, TParent>(TChild value) where TChild : TParent
   {
      return new Match<TParent>(value);
   }

   [Obsolete]
   public static Matched<TParent> matchedAs<TChild, TParent>(TChild value) where TChild : class where TParent : class
   {
      return new Match<TParent>(value as TParent);
   }

   [Obsolete]
   public static Matched<T> noMatch<T>() => new NoMatch<T>();

   [Obsolete]
   public static Matched<T> failedMatch<T>(Exception exception) => new FailedMatch<T>(exception);

   [Obsolete]
   public static Matched<T> isMatched<T>(bool test, Func<T> result)
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

   [Obsolete]
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

   [Obsolete("Use exception")]
   public static Optional<T> noResponse<T>() => new Empty<T>();

   public static Optional<T> maybe<T>(bool test, Func<T> ifTrue) => test ? ifTrue().Some() : nil;

   public static Optional<T> maybe<T>(bool test, Func<Optional<T>> ifTrue) => test ? ifTrue() : nil;

   [Obsolete("Use nil")]
   public static Completion<T> cancelled<T>() => new Cancelled<T>();

   [Obsolete("Use exception")]
   public static Completion<T> interrupted<T>(Exception exception) => new Interrupted<T>(exception);

   public static Optional<T> assert<T>(bool test, Func<T> ifTrue, Func<string> ifFalse)
   {
      try
      {
         return test ? ifTrue().Success() : ifFalse().Failure<T>();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Optional<T> assert<T>(bool test, Func<Optional<T>> ifTrue, Func<string> ifFalse)
   {
      try
      {
         return test ? ifTrue() : ifFalse().Failure<T>();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Optional<Unit> assert(bool test, Func<string> ifFalse)
   {
      try
      {
         return test ? unit : ifFalse().Failure<Unit>();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Optional<T>.If maybe<T>() => new(true);

   public static Optional<T>.If result<T>() => new(true, nil, nil);
}