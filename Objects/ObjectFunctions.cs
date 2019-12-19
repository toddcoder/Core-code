﻿using System;
using System.Linq.Expressions;
using Core.Assertions;

namespace Core.Objects
{
	public static class ObjectFunctions
	{
		public static void swap<T>(ref T left, ref T right)
		{
			var temp = left;
			left = right;
			right = temp;
		}

      public static string memberName<T>(Expression<Func<T>> memberExpression)
      {
         memberExpression.MustAs(nameof(memberExpression)).Not.BeNull().Assert();

         var expressionBody = (MemberExpression)memberExpression.Body;
         return expressionBody.Member.Name;
      }
	}
}