using System;
using System.Linq;
using Core.Computers;
using Core.Dates;
using Core.Monads;
using Core.Objects;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.ObjectGraphs.Configurations.Json.JsonDeserializerFunctions;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class JsonDeserializer<T>
   {
      public static IResult<T> Deserialize(JsonObject obj) => DeserializeObject(typeof(T), obj).Map(o => (T)o);

      protected string json;

      public JsonDeserializer(string json) => this.json = json;

      public IResult<T> Deserialize(params object[] args) =>
         from obj in typeof(T).TryCreate(args)
         from jsonObject in json.JsonObject()
         from filled in FillObject(obj, jsonObject)
         select (T)obj;
   }

   public static class JsonDeserializerFunctions
   {
      public static IResult<object> DeserializeObject(Type type, JsonObject jsonObject) =>
         from obj in type.TryCreate()
         from filled in FillObject(obj, jsonObject)
         select obj;

      public static IResult<object> GetMember(Type type, JsonBase jsonObject)
      {
         object result;
         switch (jsonObject)
         {
            case JsonString js:
               result = GetFromString(type, js.ToString());
               break;
            case JsonBoolean jb:
               result = jb.ToBoolean();
               break;
            case JsonInteger ji:
               result = ji.ToInteger();
               break;
            case JsonDouble jd:
               result = jd.ToDouble();
               break;
            case JsonArray ja:
               if (GetArray(type, ja).If(out var array, out var exception))
               {
                  result = array;
               }
               else
               {
                  return failure<object>(exception);
               }

               break;
            case JsonObject jo:
               if (DeserializeObject(type, jo).If(out var obj, out exception))
               {
                  result = obj;
               }
               else
               {
                  return failure<object>(exception);
               }

               break;
            default:
               return $"Didn't understand {jsonObject}".Failure<object>();
         }

         return result.Success();
      }

      public static IResult<object> GetFromString(Type type, string value)
      {
         if (type == typeof(Guid))
         {
            return value.Guid().Map(g => (object)g);
         }

         if (type == typeof(FileName))
         {
            return FileName.Try.FromString(value).Map(f => (object)f);
         }

         if (type == typeof(FolderName))
         {
            return FolderName.Try.FromString(value).Map(f => (object)f);
         }

         if (type == typeof(byte[]))
         {
            return tryTo(value.FromBase64).Map(b => (object)b);
         }

         if (type.IsEnum)
         {
            return tryTo(() => GetEnumeration(value, type));
         }

         if (type == typeof(TimeSpan))
         {
            return value.TimeSpan().Map(ts => (object)ts);
         }

         return ((object)value).Success();
      }

      public static object GetEnumeration(string name, Type enumerationType)
      {
         var modifiedName = name.Substitute("^ .* '.' /(.*) $", "$1");
         return (Enum)Enum.Parse(enumerationType, modifiedName);
      }

      public static IResult<Unit> FillObject(object obj, JsonObject jsonObject)
      {
         var evaluator = new PropertyEvaluator(obj);
         foreach (var member in jsonObject)
         {
            var signature = member.Name.ToUpper1();
            if (evaluator.Contains(signature))
            {
               var type = evaluator.Type(signature);
               if (GetMember(type, member).If(out obj, out var exception))
               {
                  evaluator[signature] = obj;
               }
               else
               {
                  return failure<Unit>(exception);
               }
            }
         }

         return Unit.Success();
      }

      public static IResult<object> GetArray(Type type, JsonArray array)
      {
         var elementType = type.GetElementType();
         var elements = array.ToArray();
         return
            from deserialized in elements.Select(element => GetMember(elementType, element)).IfAllSuccesses()
            from deserializedArray in tryTo(deserialized.ToArray)
            from instance in tryTo(() => Array.CreateInstance(elementType, elements.Length))
            from filledArray in tryTo(() =>
            {
               deserializedArray.CopyTo(instance, 0);
               return instance;
            })
            select (object)filledArray;
      }
   }
}