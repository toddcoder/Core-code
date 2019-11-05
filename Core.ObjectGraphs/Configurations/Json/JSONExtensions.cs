using System.Linq;
using Core.Collections;
using Core.Computers;
using Core.Monads;

namespace Core.ObjectGraphs.Configurations.Json
{
   public static class JsonExtensions
   {
      public static IResult<JsonObject> JsonObject(this string source)
      {
         var parser = new JsonParser(source);
         return parser.Parse();
      }

      public static IResult<T> ObjectFromJson<T>(this string source, params object[] args)
      {
         var deserializer = new JsonDeserializer<T>(source);
         return deserializer.Deserialize(args);
      }

      public static IResult<T> ObjectFromJson<T>(this FileName file, params object[] args)
      {
         return file.TryTo.Text.Map(text => text.ObjectFromJson<T>(args));
      }

      public static IResult<JsonObject> JsonObject(this string source, Hash<string, string> replacements)
      {
         var parser = new JsonParser(source, replacements);
         return parser.Parse();
      }

      public static IResult<JsonObject> JsonObject(this FileName file)
      {
         return file.TryTo.Text.Map(source => source.JsonObject());
      }

      public static IResult<JsonObject> JsonObject(this FileName file, Hash<string, string> replacements)
      {
         return file.TryTo.Text.Map(source => source.JsonObject(replacements));
      }

      public static IResult<Hash<string, string>> Replacements(this JsonObject obj)
      {
         var hash = new Hash<string, string>();

         foreach (var aString in obj.Select(item => item.String()))
         {
            if (aString.ValueOrCast<Hash<string, string>>(out var str, out var asHash))
            {
               hash[str.Name] = str.ToString();
            }
            else
            {
               return asHash;
            }
         }

         return hash.Success();
      }

      public static IResult<T> Deserialize<T>(this JsonObject obj) => JsonDeserializer<T>.Deserialize(obj);

      public static IResult<string> Serialize(this object obj) =>
         from serializer in JsonSerializer.New(obj)
         from result in serializer.Serialize()
         select result;
   }
}