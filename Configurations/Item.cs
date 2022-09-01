using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Configurations
{
   public class Item : IConfigurationItem
   {
      public Item(string key, string value)
      {
         Key = key;
         Value = value;
      }

      public string Key { get; }

      public string this[string key]
      {
         get => key == Key ? Value : string.Empty;
         set { }
      }

      public bool IsArray { get; set; }

      public int Indentation { get; set; }

      public IConfigurationItem GetItem(string key) => this;

      public Maybe<IConfigurationItem> GetSomeItem(string key) => maybe(key == Key, () => (IConfigurationItem)this);

      public void SetItem(string key, IConfigurationItem item)
      {
      }

      public Maybe<string> GetValue(string key) => maybe(key.Same(Key), () => Value);

      public string ValueAt(string key) => GetValue(key).Required($"Couldn't find value '{key}'");

      public IEnumerable<(string key, string value)> Values() => Enumerable.Empty<(string, string)>();

      public string At(string key) => GetValue(key) | "";

      public Result<string> RequireValue(string key) => assert(key.Same(Key), () => Value, () => $"Key '{key}' doesn't match item key");

      public Maybe<Group> GetGroup(string key) => nil;

      public Group GroupAt(string key) => GetGroup(key).Required($"Couldn't find group at '{key}'");

      public IEnumerable<(string key, Group group)> Groups() => Enumerable.Empty<(string, Group)>();

      public int Count => 1;

      public Result<Group> RequireGroup(string key) => fail("Not a group");

      public string Value { get; }

      public override string ToString()
      {
         var value = Value.ReplaceAll(("\t", @"`t"), ("\r", @"`r"), ("\n", @"`n"));
         if (value.IsEmpty())
         {
            return $"{Key}: \"{value}\"";
         }
         else if (IsArray)
         {
            var destringifier = DelimitedText.AsSql();
            var destringified = destringifier.Destringify(value);
            var array = destringified.Unjoin("/s* ',' /s*; f").Select(i => destringifier.Restringify(i, RestringifyQuotes.DoubleQuote));

            using var writer = new StringWriter();
            writer.WriteLine($"{Key}: {{");
            var indentation = " ".Repeat((Indentation + 1) * 3);
            foreach (var item in array)
            {
               writer.WriteLine($"{indentation}{item}");
            }

            writer.Write("}");

            return writer.ToString();
         }
         else if (value.StartsWith(@"""") && value.EndsWith(@""""))
         {
            var innerValue = value.Drop(1).Drop(-1).Replace(@"""", @"\""");
            return $"{Key}: \"{innerValue}\"";
         }
         else
         {
            return $"{Key}: {value}";
         }
      }
   }
}