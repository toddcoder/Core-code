using System;

namespace Core.Monads
{
   public static class MonadFunctions
   {
      public static IMaybe<TParent> some<TChild, TParent>(TChild value)
         where TChild : TParent
      {
         return new Some<TParent>(value);
      }

      public static IMaybe<TParent> someAs<TChild, TParent>(TChild value)
         where TChild : class, TParent
         where TParent : class
      {
         return new Some<TParent>(value);
      }

      public static IMaybe<T> none<T>() => new None<T>();

      public static IResult<T> success<T>(T value) => new Success<T>(value);

      public static IResult<TParent> successAs<TChild, TParent>(TChild value)
         where TChild : class, TParent
         where TParent : class
      {
         return new Success<TParent>(value);
      }

      public static IResult<T> failure<T>(Exception exception) => new Failure<T>(exception);

      public static IResult<T> failure<T>() => new Failure<T>(new Exception("Default setting"));

      public static IMatched<TParent> matched<TChild, TParent>(TChild value)
         where TChild : TParent
      {
         return new Matched<TParent>(value);
      }

      public static IMatched<TParent> matchedAs<TChild, TParent>(TChild value)
         where TChild : class
         where TParent : class
      {
         return new Matched<TParent>(value as TParent);
      }

      public static IMatched<T> notMatched<T>() => new NotMatched<T>();

      public static IMatched<T> failedMatch<T>(Exception exception) => new FailedMatch<T>(exception);

      public static IMatched<T> ifMatches<T>(bool test, Func<T> result)
      {
         try
         {
            return test ? result().Matched() : notMatched<T>();
         }
         catch (Exception exception)
         {
            return failedMatch<T>(exception);
         }
      }

      public static IMatched<T> ifMatches<T>(bool test, Func<IMatched<T>> result)
      {
         try
         {
            if (test)
            {
               return result();
            }

            return notMatched<T>();
         }
         catch (Exception exception)
         {
            return failedMatch<T>(exception);
         }
      }

      public static IMaybe<T> maybe<T>(bool test, Func<T> ifTrue) => test ? ifTrue().Some() : none<T>();

      public static IMaybe<T> maybe<T>(bool test, Func<IMaybe<T>> ifTrue) => test ? ifTrue() : none<T>();
   }
}