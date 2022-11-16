using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Configurations;

public class Item : ConfigurationItem, IConfigurationItemGetter
{
   public Item(string key, string text)
   {
      Key = key;
      Text = text;
   }

   public override string Key { get; }

   public string Text { get; }

   Maybe<Setting> IConfigurationItemGetter.GetSetting(string key) => nil;

   Maybe<Item> IConfigurationItemGetter.GetItem(string key) => nil;

   public override void SetItem(string key, ConfigurationItem item)
   {
   }

   public override IEnumerable<(string key, string text)> Items()
   {
      yield break;
   }

   public override IEnumerable<(string key, Setting setting)> Settings()
   {
      yield break;
   }

   public override int Count => 0;

   public string this[string key] => key == Key ? Text : string.Empty;

   public bool IsArray { get; set; }

   public int Indentation { get; set; }

   public override string ToString()
   {
      var value = Text.ReplaceAll(("\t", @"`t"), ("\r", @"`r"), ("\n", @"`n"));
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