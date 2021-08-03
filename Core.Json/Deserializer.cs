using System;
using System.Text;
using System.Text.Json;
using Core.Configurations;
using Core.DataStructures;
using Core.Monads;
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

      public Result<Configuration> Deserialize()
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

         var _propertyName = none<string>();

         void setItem(string value)
         {
            var item =
               from _group in peekGroup()
               from _key in _propertyName
               select (_group, _key);
            if (item.If(out var group, out var key))
            {
               group[key] = new Item(key, value);
            }

            _propertyName = none<string>();
         }

         try
         {
            var utf8 = Encoding.UTF8;
            var options = new JsonReaderOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
            var reader = new Utf8JsonReader(utf8.GetBytes(source), options);
            var firstObjectProcessed = false;

            while (reader.Read())
            {
               switch (reader.TokenType)
               {
                  case JsonTokenType.StartObject when firstObjectProcessed:
                  {
                     var key = _propertyName.DefaultTo(() => $"${itemCount()}");
                     _propertyName = none<string>();
                     var group = new Group(key);
                     if (peekGroup().If(out var parentGroup))
                     {
                        parentGroup[key] = group;
                     }
                     else
                     {
                        return "No parent group found".Failure<Configuration>();
                     }

                     stack.Push(group);
                     break;
                  }
                  case JsonTokenType.StartObject:
                     firstObjectProcessed = true;
                     break;
                  case JsonTokenType.EndObject:
                     if (stack.IsEmpty)
                     {
                        return "No parent group available".Failure<Configuration>();
                     }

                     stack.Pop();
                     break;
                  case JsonTokenType.StartArray:
                  {
                     var key = _propertyName.DefaultTo(() => $"${itemCount()}");
                     _propertyName = none<string>();
                     var group = new Group(key);
                     if (peekGroup().If(out var parentGroup))
                     {
                        parentGroup[group.Key] = group;
                     }
                     else
                     {
                        return "No parent group found".Failure<Configuration>();
                     }

                     stack.Push(group);

                     break;
                  }
                  case JsonTokenType.EndArray:
                     if (stack.IsEmpty)
                     {
                        return "No parent group available".Failure<Configuration>();
                     }

                     stack.Pop();
                     break;
                  case JsonTokenType.PropertyName:
                     _propertyName = reader.GetString().Some();
                     break;
                  case JsonTokenType.String:
                     setItem(reader.GetString());
                     break;
                  case JsonTokenType.Number:
                     setItem(reader.GetDouble().ToString());
                     break;
                  case JsonTokenType.True:
                     setItem("true");
                     break;
                  case JsonTokenType.False:
                     setItem("false");
                     break;
               }
            }

            return new Configuration(rootGroup).Success();
         }
         catch (Exception exception)
         {
            return failure<Configuration>(exception);
         }
      }
   }
}