using System;
using System.Threading;
using System.Threading.Tasks;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
	public static class AttemptFunctions
	{
		public static IResult<T> tryTo<T>(Func<T> func)
		{
			try
			{
				return func().Success();
			}
			catch (Exception exception)
			{
				return failure<T>(exception);
			}
		}

		public static IResult<T> tryTo<T>(Func<IResult<T>> func)
		{
			try
			{
				return func();
			}
			catch (Exception exception)
			{
				return failure<T>(exception);
			}
		}

		public static IResult<Unit> tryTo(Action action)
		{
			try
			{
				action();
				return Unit.Success();
			}
			catch (Exception exception)
			{
				return failure<Unit>(exception);
			}
		}

		public static IResult<T> assert<T>(bool test, Func<T> ifTrue, string messageIfFalse)
		{
			try
			{
				return test ? ifTrue().Success() : messageIfFalse.Failure<T>();
			}
			catch (Exception exception)
			{
				return failure<T>(exception);
			}
		}

		public static IResult<T> assert<T>(Func<bool> test, Func<T> ifTrue, string messageIfFalse)
		{
			try
			{
				return test() ? ifTrue().Success() : messageIfFalse.Failure<T>();
			}
			catch (Exception exception)
			{
				return failure<T>(exception);
			}
		}

		public static IResult<T> assert<T>(bool test, Func<T> ifTrue, Func<string> messageIfFalse)
		{
			try
			{
				return test ? ifTrue().Success() : messageIfFalse().Failure<T>();
			}
			catch (Exception exception)
			{
				return failure<T>(exception);
			}
		}

		public static IResult<T> assert<T>(Func<bool> test, Func<T> ifTrue, Func<string> messageIfFalse)
		{
			try
			{
				return test() ? ifTrue().Success() : messageIfFalse().Failure<T>();
			}
			catch (Exception exception)
			{
				return failure<T>(exception);
			}
		}

		public static IResult<T> assert<T>(bool test, Func<IResult<T>> ifTrue, string messageIfFalse)
		{
			try
			{
				return test ? ifTrue() : messageIfFalse.Failure<T>();
			}
			catch (Exception exception)
			{
				return failure<T>(exception);
			}
		}

		public static IResult<T> assert<T>(Func<bool> test, Func<IResult<T>> ifTrue, string messageIfFalse)
		{
			try
			{
				return test() ? ifTrue() : messageIfFalse.Failure<T>();
			}
			catch (Exception exception)
			{
				return failure<T>(exception);
			}
		}

		public static IResult<T> assert<T>(bool test, Func<IResult<T>> ifTrue, Func<string> messageIfFalse)
		{
			try
			{
				return test ? ifTrue() : messageIfFalse().Failure<T>();
			}
			catch (Exception exception)
			{
				return failure<T>(exception);
			}
		}

		public static IResult<T> assert<T>(Func<bool> test, Func<IResult<T>> ifTrue, Func<string> messageIfFalse)
		{
			try
			{
				return test() ? ifTrue() : messageIfFalse().Failure<T>();
			}
			catch (Exception exception)
			{
				return failure<T>(exception);
			}
		}

      public static IResult<T> assertNotNull<T>(T obj, Func<string> messageIfNull)
      {
         try
         {
            if (obj != null)
            {
               return obj.Success();
            }
            else
            {
               return messageIfNull().Failure<T>();
            }
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

      public static IResult<T> assertNotNull<T>(T obj, string messageIfNull)
      {
         try
         {
            if (obj != null)
            {
               return obj.Success();
            }
            else
            {
               return messageIfNull.Failure<T>();
            }
         }
         catch (Exception exception)
         {
            return failure<T>(exception);
         }
      }

		public static IResult<T> reject<T>(bool test, Func<T> ifFalse, string messageIfTrue)
		{
			return assert(!test, ifFalse, messageIfTrue);
		}

		public static IResult<T> reject<T>(Func<bool> test, Func<T> ifFalse, string messageIfTrue)
		{
			return tryTo(() => assert(!test(), ifFalse, messageIfTrue));
		}

		public static IResult<T> reject<T>(bool test, Func<T> ifFalse, Func<string> messageIfTrue)
		{
			return assert(!test, ifFalse, messageIfTrue);
		}

		public static IResult<T> reject<T>(Func<bool> test, Func<T> ifFalse, Func<string> messageIfTrue)
		{
			return tryTo(() => assert(!test(), ifFalse, messageIfTrue));
		}

		public static IResult<T> reject<T>(bool test, Func<IResult<T>> ifFalse, string messageIfTrue)
		{
			return assert(!test, ifFalse, messageIfTrue);
		}

		public static IResult<T> reject<T>(Func<bool> test, Func<IResult<T>> ifFalse, string messageIfTrue)
		{
			return tryTo(() => assert(!test(), ifFalse, messageIfTrue));
		}

		public static IResult<T> reject<T>(bool test, Func<IResult<T>> ifFalse, Func<string> messageIfTrue)
		{
			return assert(!test, ifFalse, messageIfTrue);
		}

		public static IResult<T> reject<T>(Func<bool> test, Func<IResult<T>> ifFalse, Func<string> messageIfTrue)
		{
			return tryTo(() => assert(!test(), ifFalse, messageIfTrue));
		}

		public static IResult<Unit> attempt(Action<int> action, int attempts)
		{
			var result = "Action to try hasn't been executed".Failure<Unit>();

			for (var attempt = 0; attempt < attempts; attempt++)
			{
				result = tryTo(() => action(attempt));
				if (result.IsSuccessful)
            {
               return result;
            }
         }

			return result;
		}

      public static async Task<ICompletion<T>> run<T>(Func<CancellationToken, ICompletion<T>> func, CancellationTokenSource source)
      {
         try
         {
            var token = source.Token;
            return await Task.Run(() => func(token), token);
         }
         catch (OperationCanceledException)
         {
            return cancelled<T>();
         }
         catch (Exception exception)
         {
            return interrupted<T>(exception);
         }
      }
	}
}