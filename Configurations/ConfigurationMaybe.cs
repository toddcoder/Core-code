using System;
using System.Linq;
using Core.Computers;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Configurations;

public class ConfigurationMaybe
{
   protected IConfigurationItemGetter getter;

   internal ConfigurationMaybe(IConfigurationItemGetter getter)
   {
      this.getter = getter;
   }

   public Optional<Setting> Setting(string key) => getter.GetSetting(key);

   public Optional<Item> Item(string key) => getter.GetItem(key);

   public Optional<string> String(string key) => getter.GetItem(key).Map(i => i.Text);

   public Optional<int> Int32(string key) => String(key).Map(i => Maybe.Int32(i));

   public Optional<long> Int64(string key) => String(key).Map(l => Maybe.Int64(l));

   public Optional<float> Single(string key) => String(key).Map(s => Maybe.Single(s));

   public Optional<double> Double(string key) => String(key).Map(d => Maybe.Double(d));

   public Optional<bool> Boolean(string key) => String(key).Map(Maybe.Boolean);

   public Optional<DateTime> DateTime(string key) => String(key).Map(Maybe.DateTime);

   public Optional<Guid> Guid(string key) => String(key).Map(Maybe.Guid);

   public Optional<FileName> FileName(string key)
   {
      try
      {
         var _fileName = String(key);
         if ((_fileName | "").IsEmpty())
         {
            return nil;
         }

         var _file = _fileName.Map(s => (FileName)s);
         return _file.Map(f => f.IsValid) | false ? _file : nil;
      }
      catch
      {
         return nil;
      }
   }

   public Optional<FolderName> FolderName(string key)
   {
      try
      {
         var _folderName = String(key);
         if ((_folderName | "").IsEmpty())
         {
            return nil;
         }

         var _folder = _folderName.Map(s => (FolderName)s);
         return _folder.Map(f => f.IsValid) | false ? _folder : nil;
      }
      catch
      {
         return nil;
      }
   }

   public Optional<byte[]> Bytes(string key)
   {
      try
      {
         return String(key).Map(s => s.FromBase64());
      }
      catch
      {
         return nil;
      }
   }

   public Optional<TimeSpan> TimeSpan(string key) => String(key).Map(Maybe.TimeSpan);

   public Optional<string[]> Strings(string key) => String(key).Map(s => s.Unjoin("/s* ',' /s*"));

   public Optional<string[]> SettingTexts(string key) => Setting(key).Map(s => s.Items().Select(i => i.text).ToArray());

   public Optional<string[]> SettingKeys(string key) => Setting(key).Map(s => s.Items().Select(i => i.key).ToArray());
}