using System;
using System.Collections.Generic;
using System.IO;
using Core.Configurations;
using Core.Dates;
using Core.Monads;
using Newtonsoft.Json;
using static Core.Monads.MonadFunctions;
using static Core.Monads.Monads;

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
      var stack = new Stack<Setting>();
      stack.Push(rootSetting);
      var parentSetting = rootSetting;

      var _propertyName = monads.maybe<string>();

      string getKey(Maybe<string> _propertyName)
      {
         if (_propertyName is (true, var propertyName))
         {
            return propertyName;
         }
         else
         {
            return $"${parentSetting.Count}";
         }
      }

      void setItem(string value)
      {
         var key = getKey(_propertyName);
         parentSetting.SetItem(key, new Item(key, value));
         _propertyName = nil;
      }

      void setItemNull()
      {
         var key = getKey(_propertyName);
         parentSetting.SetItem(key, new Item(key, "") { IsNull = true });
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
                  var key = getKey(_propertyName);
                  _propertyName = nil;
                  var setting = new Setting(key);
                  parentSetting.SetItem(key, setting);

                  stack.Push(parentSetting);
                  parentSetting = setting;
                  break;
               }
               case JsonToken.StartObject:
                  firstObjectProcessed = true;
                  break;
               case JsonToken.EndObject:
                  if (stack.Count == 0)
                  {
                     return fail("No parent group available");
                  }

                  parentSetting = stack.Pop();
                  break;
               case JsonToken.StartArray:
               {
                  var key = getKey(_propertyName);
                  _propertyName = nil;
                  var setting = new Setting(key) { IsArray = true };
                  parentSetting.SetItem(setting.Key, setting);

                  stack.Push(parentSetting);
                  parentSetting = setting;

                  break;
               }
               case JsonToken.EndArray:
                  if (stack.Count == 0)
                  {
                     return fail("No parent setting available");
                  }

                  parentSetting = stack.Pop();
                  break;
               case JsonToken.PropertyName:
               {
                  var propertyName = reader.Value;
                  if (propertyName is null)
                  {
                     _propertyName = nil;
                  }
                  else
                  {
                     _propertyName = reader.Value.ToString();
                  }

                  break;
               }
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