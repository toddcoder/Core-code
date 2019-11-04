using System.Linq;
using Core.Collections;
using Core.Computers;
using Core.Monads;

namespace Core.ObjectGraphs.Configurations.Json
{
   public static class JSONExtensions
   {
      public static IResult<JSONObject> JSONObject(this string source)
      {
         var parser = new JSONParser(source);
         return parser.Parse();
      }

      public static IResult<T> ObjectFromJSON<T>(this string source, params object[] args)
      {
         var deserializer = new JSONDeserializer<T>(source);
         return deserializer.Deserialize(args);
      }

      public static IResult<T> ObjectFromJSON<T>(this FileName file, params object[] args)
      {
         return file.TryTo.Text.Map(text => text.ObjectFromJSON<T>(args));
      }

      public static IResult<JSONObject> JSONObject(this string source, Hash<string, string> replacements)
      {
         var parser = new JSONParser(source, replacements);
         return parser.Parse();
      }

      public static IResult<JSONObject> JSONObject(this FileName file)
      {
         return file.TryTo.Text.Map(source => source.JSONObject());
      }

      public static IResult<JSONObject> JSONObject(this FileName file, Hash<string, string> replacements)
      {
         return file.TryTo.Text.Map(source => source.JSONObject(replacements));
      }

      public static IResult<Hash<string, string>> Replacements(this JSONObject obj)
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

      public static IResult<T> Deserialize<T>(this JSONObject obj) => JSONDeserializer<T>.Deserialize(obj);

      public static IResult<string> Serialize(this object obj) =>
         from serializer in JSONSerializer.New(obj)
         from result in serializer.Serialize()
         select result;
   }
}