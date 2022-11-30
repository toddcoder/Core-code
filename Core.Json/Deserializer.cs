using System;
using System.IO;
using Core.Configurations;
using Core.DataStructures;
using Core.Dates;
using Core.Monads;
using Core.Strings;
using Newtonsoft.Json;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Json;

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
         var _setting = peekSetting();
         if (_setting)
         {
            (~_setting).SetItem(key, new Item(key, value));
         }

         _propertyName = nil;
      }

      void setItemNull()
      {
         var key = _propertyName | (() => $"${itemCount()}");
         var _setting = peekSetting();
         if (_setting)
         {
            (~_setting).SetItem(key, new Item(key, "") { IsNull = true });
         }

         _propertyName = nil;
      }

      try
      {
         using var textReader = new StringReader(source);
         using var reader = new JsonTextReader(textReader);
         reader.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
         var firstObjectProcessed = false;

         string getValue() => reader.Value?.ToString() ?? "";

         string getDateTime()
         {
            var dateTime = (DateTime)reader.Value;
            return dateTime.Zulu();
         }

         while (reader.Read())
         {
            switch (reader.TokenType)
            {
               case JsonToken.StartObject when firstObjectProcessed:
               {
                  var key = _propertyName | (() => $"${itemCount()}");
                  _propertyName = nil;
                  var setting = new Setting(key);
                  var _parentSetting = peekSetting();
                  if (_parentSetting)
                  {
                     (~_parentSetting).SetItem(key, setting);
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
                  var setting = new Setting(key) { IsArray = true };
                  var _parentSetting = peekSetting();
                  if (_parentSetting)
                  {
                     (~_parentSetting).SetItem(setting.Key, setting);
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
                  _propertyName = reader.Value.NotNull().Map(o => o.ToNonNullString());
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
                  setItem(getDateTime());
                  break;
               case JsonToken.Null:
                  setItemNull();
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