using System;
using Core.Monads;
using Core.RegularExpressions;
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
      public static IMaybe<object> DefaultValue(this Type type)
      {
         if (type != null)
         {
            var expression = Lambda<Func<object>>(Convert(Default(type), typeof(object)));
            return expression.Compile()().Some();
         }
         else
         {
            return none<object>();
         }
      }

      public static IMaybe<object> DefaultValue(this string typeName, bool defaultStringToEmpty = false)
      {
         if (typeName.StartsWith("$"))
         {
            typeName = "System." + typeName.Tail();
         }

         if (defaultStringToEmpty && typeName.IsMatch("^ 'System.string' $", true))
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
         if (type != null)
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

      public static IResult<Type> TypeOf(this string source)
      {
         try
         {
            if (source.MatchOne("^ -/{,} ','? /s* /{a-zA-Z_0-9.} $").If(out var m))
            {
               return getUngenericType(m.FirstGroup, m.SecondGroup).Success();
            }
            else if (source.MatchOne("^ -/{,} ','? /s* /{a-zA-Z_0-9.} '<' -/{,} ',' -/{>} '>' $").If(out m))
            {
               return getGenericType(m.FirstGroup, m.SecondGroup, m.ThirdGroup, m.FourthGroup).Success();
            }
            else
            {
               return Type.GetType(source).Success();
            }
         }
         catch (Exception exception)
         {
            return failure<Type>(exception);
         }
      }

      static Type getUngenericType(string assemblyPath, string typeName)
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

      static Type getGenericType(string genericAssemblyPath, string genericTypeName, string specificAssemblyPath,
         string specificTypeName)
      {
         var specificType = getUngenericType(specificAssemblyPath, specificTypeName);
         if (specificType != null)
         {
            var fullTypeName = $"{genericTypeName}`[[{specificType.AssemblyQualifiedName}]]";
            return LoadFrom(genericAssemblyPath).GetType(fullTypeName, false);
         }
         else
         {
            return null;
         }
      }

      public static IResult<object> New(this Type type, params object[] args) => tryTo(() => Activator.CreateInstance(type, array(args)));
   }
}