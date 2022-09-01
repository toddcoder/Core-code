﻿using System;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static System.Reflection.Assembly;
using static System.Linq.Expressions.Expression;
using static Core.Arrays.ArrayFunctions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Objects
{
   public static class TypeExtensions
   {
      public static Maybe<object> DefaultValue(this Type type)
      {
         return maybe(type is not null, () =>
         {
            var expression = Lambda<Func<object>>(Convert(Default(type), typeof(object)));
            return expression.Compile()();
         });
      }

      public static Maybe<object> DefaultValue(this string typeName, bool defaultStringToEmpty = false)
      {
         if (typeName.StartsWith("$"))
         {
            typeName = $"System.{typeName.Tail()}";
         }

         if (defaultStringToEmpty && typeName.IsMatch("^ 'System.string' $; fi"))
         {
            return some<string, object>("");
         }
         else
         {
            return Type.GetType(typeName, false, true).DefaultValue();
         }
      }

      public static object DefaultOf(this Type type)
      {
         if (type is not null)
         {
            try
            {
               var expression = Lambda<Func<object>>(Convert(Default(type), typeof(object)));
               return expression.Compile()();
            }
            catch
            {
               return null;
            }
         }
         else
         {
            return null;
         }
      }

      public static Result<Type> TypeOf(this string source)
      {
         try
         {
            if (source.Matches("^ -/{,} ','? /s* /{a-zA-Z_0-9.} $; f").Map(out var result))
            {
               return getUngenericType(result.FirstGroup, result.SecondGroup).Success();
            }
            else if (source.Matches("^ -/{,} ','? /s* /{a-zA-Z_0-9.} '<' -/{,} ',' -/{>} '>' $; f").Map(out result))
            {
               return getGenericType(result.FirstGroup, result.SecondGroup, result.ThirdGroup, result.FourthGroup).Success();
            }
            else
            {
               return Type.GetType(source).Success();
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      private static Type getUngenericType(string assemblyPath, string typeName)
      {
         if (assemblyPath.IsEmpty())
         {
            return Type.GetType(typeName.ToTitleCase().Replace("$", "System"), false);
         }
         else
         {
            return LoadFrom(assemblyPath).GetType(typeName);
         }
      }

      private static Type getGenericType(string genericAssemblyPath, string genericTypeName, string specificAssemblyPath, string specificTypeName)
      {
         var specificType = getUngenericType(specificAssemblyPath, specificTypeName);
         if (specificType is not null)
         {
            var fullTypeName = $"{genericTypeName}`[[{specificType.AssemblyQualifiedName}]]";
            return LoadFrom(genericAssemblyPath).GetType(fullTypeName, false);
         }
         else
         {
            return null;
         }
      }

      public static Result<object> New(this Type type, params object[] args) => tryTo(() => Activator.CreateInstance(type, array(args)));
   }
}