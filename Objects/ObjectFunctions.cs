using System;
using System.Linq.Expressions;
using Core.Assertions;

namespace Core.Objects;

public static class ObjectFunctions
{
   public static void swap<T>(ref T left, ref T right)
   {
      (left, right) = (right, left);
   }

   public static string memberName<T>(Expression<Func<T>> memberExpression)
   {
      memberExpression.Must().Not.BeNull().OrThrow();

      var expressionBody = (MemberExpression)memberExpression.Body;
      return expressionBody.Member.Name;
   }
}