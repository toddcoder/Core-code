using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ReflectorFormat;

namespace Core.Objects
{
   public static class ObjectExtensions
   {
      public static bool IsNullable(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

      public static bool IsNull<T>(this T obj) => !typeof(T).IsValueType && EqualityComparer<T>.Default.Equals(obj, default);

      public static bool IsNotNull<T>(this T obj) => !obj.IsNull();

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

      public static IResult<T> CastAs<T>(this object obj) => tryTo(() =>
      {
         if (obj is T o)
         {
            return o.Success();
         }
         else
         {
            return $"{obj} can't be cast to {typeof(T).FullName}".Failure<T>();
         }
      });

      public static IMatched<T> MatchAs<T>(this object obj)
      {
         try
         {
            if (obj is T o)
            {
               return o.Matched();
            }
            else
            {
               return notMatched<T>();
            }
         }
         catch (Exception exception)
         {
            return failedMatch<T>(exception);
         }
      }

      public static string GUID(this Guid value) => value.ToString().ToUpper();

      public static string Compressed(this Guid value) => value.GUID().Substitute("['{}-']", "");

      public static string WithoutBrackets(this Guid value) => value.GUID().Drop(1).Drop(-1);

      public static bool IsDate(this object date)
      {
         if (date is DateTime)
         {
            return true;
         }
         else
         {
            return DateTime.TryParse(date.ToString(), out _);
         }
      }

      public static IResult<string> FormatObject(this object obj, string format) => GetReflector(obj).Map(rf => rf.Format(format));

      public static string FormatAs(this object obj, string format)
      {
         if (obj is DateTime dateTime)
         {
            return dateTime.ToString(format);
         }
         else
         {
            return format.MatchOne("/['cdefgnprxs'] /('-'? /d+)? ('.' /(/d+))?", true).FlatMap(match =>
            {
               var (specifier, width, places) = match.Groups3();

               var result = new StringBuilder("{0");
               if (width.IsNotEmpty())
               {
                  result.Append($",{width}");
               }

               if (specifier.IsNotEmpty() && specifier != "s")
               {
                  result.Append($":{specifier}");
                  if (places.IsNotEmpty())
                  {
                     result.Append(places);
                  }
               }

               result.Append("}");
               return string.Format(result.ToString(), obj);
            }, obj.ToString);
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
   }
}