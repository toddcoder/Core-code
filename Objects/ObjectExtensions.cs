using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ReflectorFormat;

namespace Core.Objects
{
   public static class ObjectExtensions
   {
      public static bool IsNullable(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

      public static bool AnyNull(this ITuple tuple)
      {
         for (var i = 0; i < tuple.Length; i++)
         {
            if (tuple[i] is null)
            {
               return true;
            }
         }

         return false;
      }

      [Obsolete("Use is null construct")]
      public static bool IsNull<T>(this T obj)
      {
         if (obj is ITuple tuple)
         {
            for (var i = 0; i < tuple.Length; i++)
            {
               if (tuple[i] is null)
               {
                  return true;
               }
            }

            return false;
         }
         else
         {
            return !typeof(T).IsValueType && EqualityComparer<T>.Default.Equals(obj, default);
         }
      }

      [Obsolete("Use is not null construct")]
      public static bool IsNotNull<T>(this T obj) => obj is not null;

      public static Type UnderlyingType(this Type type) => type.IsNullable() ? Nullable.GetUnderlyingType(type) : type;

      public static int HashCode(this object obj, int prime = 397)
      {
         var evaluator = new PropertyEvaluator(obj);
         unchecked
         {
            return evaluator.Signatures.Aggregate(prime, (current, signature) => current * prime + evaluator[signature]?.GetHashCode() ?? 0);
         }
      }

      public static int HashCode(this object obj, string signature) => PropertyEvaluator.GetValue(obj, signature).GetHashCode();

      public static Result<T> CastAs<T>(this object obj)
      {
         try
         {
            if (obj is T o)
            {
               return o;
            }
            else
            {
               return fail($"{obj} can't be cast to {typeof(T).FullName}");
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public static Responding<T> RespondingAs<T>(this object obj)
      {
         try
         {
            if (obj is T o)
            {
               return o;
            }
            else
            {
               return nil;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public static string GUID(this Guid value) => value.ToString().ToUpper();

      public static string Compressed(this Guid value) => value.GUID().Substitute("['{}-']; f", "");

      public static string WithoutBrackets(this Guid value) => value.GUID().Drop(1).Drop(-1);

      public static bool IsDate(this object date)
      {
         return date is DateTime || DateTime.TryParse(date.ToString(), out _);
      }

      public static Result<string> FormatObject(this object obj, string format) => GetReflector(obj).Map(rf => rf.Format(format));

      public static string FormatAs(this object obj, string format)
      {
         if (obj is DateTime dateTime)
         {
            return dateTime.ToString(format);
         }
         else if (format.Matches("/['cdefgnprxs'] /('-'? /d+)? ('.' /(/d+))?; fi").Map(out var result))
         {
            var (specifier, width, places) = result;

            var builder = new StringBuilder("{0");
            if (width.IsNotEmpty())
            {
               builder.Append($",{width}");
            }

            if (specifier.IsNotEmpty() && specifier != "s")
            {
               builder.Append($":{specifier}");
               if (places.IsNotEmpty())
               {
                  builder.Append(places);
               }
            }

            builder.Append("}");
            return string.Format(builder.ToString(), obj);
         }
         else
         {
            return obj.ToString();
         }
      }

      public static T RequiredCast<T>(this object obj, Func<string> message)
      {
         try
         {
            return (T)obj;
         }
         catch (Exception exception)
         {
            var formatter = new Formatter { ["object"] = obj?.ToString() ?? "", ["e"] = exception.Message };
            throw new ApplicationException(formatter.Format(message()));
         }
      }

      public static IEnumerable<(PropertyInfo propertyInfo, TAttribute attribute)> PropertiesUsing<TAttribute>(this object obj, bool inherit = true)
         where TAttribute : Attribute
      {
         if (obj is null)
         {
            yield break;
         }

         foreach (var propertyInfo in obj.GetType().GetProperties())
         {
            foreach (var userAttribute in propertyInfo.GetCustomAttributes(inherit))
            {
               if (userAttribute is TAttribute attribute)
               {
                  yield return (propertyInfo, attribute);
               }
            }
         }
      }

      public static IEnumerable<(MethodInfo methodInfo, TAttribute attribute)> MethodsUsing<TAttribute>(this object obj, bool inherit = true)
         where TAttribute : Attribute
      {
         if (obj is null)
         {
            yield break;
         }

         foreach (var methodInfo in obj.GetType().GetMethods())
         {
            foreach (var customAttribute in methodInfo.GetCustomAttributes(inherit))
            {
               if (customAttribute is TAttribute attribute)
               {
                  yield return (methodInfo, attribute);
               }
            }
         }
      }
   }
}