using System;
using System.IO;
using Core.Configurations;
using Core.DataStructures;
using Core.Monads;
using Core.Strings;
using Newtonsoft.Json;
using static Core.Monads.MonadFunctions;

namespace Core.Json
{
   public class Deserializer
   {
      protected string source;

      public Deserializer(string source)
      {
         this.source = source;
      }

      public Result<Setting> Deserialize()
      {
         var rootSetting = new Setting("/");
         var stack = new MaybeStack<ConfigurationItem>();
         stack.Push(rootSetting);

         Maybe<Setting> peekSetting()
         {
            return
               from parentItem in stack.Peek()
               from parentGroup in parentItem.IfCast<Setting>()
               select parentGroup;
         }

         int itemCount() => peekSetting().Map(setting => setting.AnyHash().Map(h => h.Values.Count)) | 0;

         Maybe<string> _propertyName = nil;

         void setItem(string value)
         {
            var key = _propertyName | (() => $"${itemCount()}");
            if (peekSetting().Map(out var setting))
            {
               setting.SetItem(key, new Item(key, value));
            }

            _propertyName = nil;
         }

         try
         {
            using var textReader = new StringReader(source);
            using var reader = new JsonTextReader(textReader);
            var firstObjectProcessed = false;

            string getValue() => reader.Value?.ToString() ?? "";

            while (reader.Read())
            {
               switch (reader.TokenType)
               {
                  case JsonToken.StartObject when firstObjectProcessed:
                  {
                     var key = _propertyName | (() => $"${itemCount()}");
                     _propertyName = nil;
                     var setting = new Setting(key);
                     if (peekSetting().Map(out var parentSetting))
                     {
                        parentSetting.SetItem(key, setting);
                     }
                     else
                     {
                        return fail("No parent setting found");
                     }

                     stack.Push(setting);
                     break;
                  }
                  case JsonToken.StartObject:
                     firstObjectProcessed = true;
                     break;
                  case JsonToken.EndObject:
                     if (stack.IsEmpty)
                     {
                        return fail("No parent group available");
                     }

                     stack.Pop();
                     break;
                  case JsonToken.StartArray:
                  {
                     var key = _propertyName | (() => $"${itemCount()}");
                     _propertyName = nil;
                     var setting = new Setting(key);
                     if (peekSetting().Map(out var parentSetting))
                     {
                        parentSetting.SetItem(setting.Key, setting);
                     }
                     else
                     {
                        return fail("No parent setting found");
                     }

                     stack.Push(setting);

                     break;
                  }
                  case JsonToken.EndArray:
                     if (stack.IsEmpty)
                     {
                        return fail("No parent setting available");
                     }

                     stack.Pop();
                     break;
                  case JsonToken.PropertyName:
                     _propertyName = reader.Value.Some().Map(o => o.ToNonNullString());
                     break;
                  case JsonToken.String:
                     setItem(getValue());
                     break;
                  case JsonToken.Integer:
                     setItem(getValue());
                     break;
                  case JsonToken.Float:
                     setItem(getValue());
                     break;
                  case JsonToken.Boolean:
                     setItem(getValue().ToLower());
                     break;
                  case JsonToken.Date:
                     setItem(getValue());
                     break;
                  default:
                     setItem("");
                     break;
               }
            }

            return rootSetting;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
   }
}