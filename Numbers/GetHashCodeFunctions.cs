using System.Linq;

namespace Core.Numbers
{
   public static class GetHashCodeFunctions
   {
      public static int getHashCode(params object[] args)
      {
         unchecked
         {
            return args.Aggregate(17, (current, arg) => current * 29 + arg.GetHashCode());
         }
      }

      public static int largeHashCode(params object[] args)
      {
         unchecked
         {
            return args.Aggregate((int)2166136261, (current, arg) => current * 16777619 ^ arg.GetHashCode());
         }
      }
   }
}