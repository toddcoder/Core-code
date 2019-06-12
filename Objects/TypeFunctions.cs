using Core.Monads;

namespace Core.Objects
{
   public static class TypeFunctions
   {
      public static IResult<T> newObject<T>(params object[] args) => typeof(T).New(args).Map(obj => (T)obj);
   }
}