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

      public Result<Group> Deserialize()
      {
         var rootGroup = new Group("/");
         var stack = new MaybeStack<IConfigurationItem>();
         stack.Push(rootGroup);

         Maybe<Group> peekGroup()
         {
            return
               from parentItem in stack.Peek()
               from parentGroup in parentItem.IfCast<Group>()
               select parentGroup;
         }

         int itemCount() => peekGroup().Map(group => group.AnyHash().Map(h => h.Values.Count).Recover(_ => 0)).DefaultTo(() => 0);

         var _propertyName = Maybe<string>.nil;

         void setItem(string value)
         {
            var key = _propertyName.DefaultTo(() => $"${itemCount()}");
            if (peekGroup().Map(out var group))
            {
               group.SetItem(key, new Item(key, value));
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
                     var key = _propertyName.DefaultTo(() => $"${itemCount()}");
                     _propertyName = nil;
                     var group = new Group(key);
                     if (peekGroup().Map(out var parentGroup))
                     {
                        parentGroup.SetItem(key, group);
                     }
                     else
                     {
                        return fail("No parent group found");
                     }

                     stack.Push(group);
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
                     var key = _propertyName.DefaultTo(() => $"${itemCount()}");
                     _propertyName = nil;
                     var group = new Group(key);
                     if (peekGroup().Map(out var parentGroup))
                     {
                        parentGroup.SetItem(group.Key, group);
                     }
                     else
                     {
                        return fail("No parent group found");
                     }

                     stack.Push(group);

                     break;
                  }
                  case JsonToken.EndArray:
                     if (stack.IsEmpty)
                     {
                        return fail("No parent group available");
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

            return rootGroup;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
   }
}