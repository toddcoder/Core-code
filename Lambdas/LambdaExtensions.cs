using System;

namespace Core.Lambdas;

public static class LambdaExtensions
{
   public static void If(this Action action, Func<bool> ifTrue)
   {
      if (ifTrue())
      {
         action();
      }
   }

   public static void If(this Action action, Func<bool> ifTrue, Action ifFalse)
   {
      if (ifTrue())
      {
         action();
      }
      else
      {
         ifFalse();
      }
   }
}