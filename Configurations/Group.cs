using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Collections;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Configurations
{
   public class Group : IConfigurationItem, IHash<string, IConfigurationItem>
   {
      protected StringHash<IConfigurationItem> items;

      internal Group(string key)
      {
         Key = key;

         items = new StringHash<IConfigurationItem>(true);
      }

      public string Key { get; }

      public IConfigurationItem this[string key]
      {
         get => items[key];
         set => items[key] = value;
      }

      public Maybe<string> GetValue(string key) => items.Map(key).Map(i => i.GetValue(key));

      public IResult<string> RequireValue(string key) => items.Require(key).Map(i => i.RequireValue(key));

      public Maybe<Group> GetGroup(string key)
      {
         if (items.If(key, out var item) && item is Group group)
         {
            return group.Some();
         }
         else
         {
            return none<Group>();
         }
      }

      public IResult<Group> RequireGroup(string key) => GetGroup(key).Result($"Key {key} not found");

      public bool ContainsKey(string key) => items.ContainsKey(key);

      public IResult<Hash<string, IConfigurationItem>> AnyHash() => items.AsHash.Success();

      public IEnumerable<(string key, string value)> Values()
      {
         foreach (var (key, item) in items.Where(i => i.Value is Item))
         {
            yield return (key, ((Item)item).Value);
         }
      }

      public IEnumerable<(string key, Group group)> Groups()
      {
         foreach (var (key, item) in items.Where(i => i.Value is Group))
         {
            yield return (key, (Group)item);
         }
      }

      public string ToString(int indent, bool ignoreSelf = false)
      {
         string indentation() => " ".Repeat(indent * 3);

         using var writer = new StringWriter();

         if (!ignoreSelf)
         {
            writer.WriteLine($"{indentation()}{Key} [");
            indent++;
         }

         foreach (var (_, value) in items)
         {
            switch (value)
            {
               case Group group:
                  writer.Write(group.ToString(indent));
                  break;
               case Item item:
                  writer.WriteLine($"{indentation()}{item}");
                  break;
            }
         }

         if (!ignoreSelf)
         {
            indent--;
            writer.WriteLine($"{indentation()}]");
         }

         return writer.ToString();
      }

      public string ToString(bool ignoreSelf) => ToString(0, ignoreSelf);
   }
}