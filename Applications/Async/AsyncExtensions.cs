using System;
using System.Threading.Tasks;
using Core.Monads;

namespace Core.Applications.Async
{
   public static class AsyncExtensions
   {
      public static async Task<ICompletion<TResult>> Map<T, TResult>(this Task<ICompletion<T>> task, Func<T, TResult> func)
      {
         return await Task.Run(() => task.Result.Map(func));
      }

      public static async Task<ICompletion<TResult>> Map<T, TResult>(this Task<ICompletion<T>> task, Func<T, ICompletion<TResult>> func)
      {
         return await Task.Run(() => task.Result.Map(func));
      }
   }
}