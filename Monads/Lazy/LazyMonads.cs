using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class LazyMonads
{
   protected static Optional<LazyMonads> _function;

   static LazyMonads()
   {
      _function = nil;
   }

   public static LazyMonads lazy
   {
      get
      {
         if (!_function)
         {
            _function = new LazyMonads();
         }

         return _function;
      }
   }

   public LazyOptional<T> maybe<T>(Func<Optional<T>> func) => new(func);

   public LazyOptional<T> maybe<T>(Optional<T> maybe) => new(maybe);

   public LazyOptional<T> maybe<T>() => new();

   public LazyOptional<T> result<T>(Func<Optional<T>> func) => new(func);

   public LazyOptional<T> result<T>(Optional<T> result) => new(result);

   public LazyOptional<T> result<T>() => new();

   public LazyOptional<T> optional<T>(Func<Optional<T>> func) => new(func);

   public LazyOptional<T> optional<T>(Optional<T> optional) => new(optional);

   public LazyOptional<T> optional<T>() => new();

   public LazyCompletion<T> completion<T>(Func<Completion<T>> func) => new(func);

   public LazyCompletion<T> completion<T>(Completion<T> completion) => new(completion);

   public LazyCompletion<T> completion<T>() => new();
}