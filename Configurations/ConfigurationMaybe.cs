using System;
using Core.Computers;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Configurations
{
   public class ConfigurationMaybe
   {
      protected IConfigurationItemGetter getter;

      internal ConfigurationMaybe(IConfigurationItemGetter getter)
      {
         this.getter = getter;
      }

      public Maybe<Setting> Setting(string key) => getter.GetSetting(key);

      public Maybe<Item> Item(string key) => getter.GetItem(key);

      public Maybe<string> String(string key) => getter.GetItem(key).Map(i => i.Text);

      public Maybe<int> Int32(string key) => String(key).Map(Maybe.Int32);

      public Maybe<long> Int64(string key) => String(key).Map(Maybe.Int64);

      public Maybe<float> Single(string key) => String(key).Map(Maybe.Single);

      public Maybe<double> Double(string key) => String(key).Map(Maybe.Double);

      public Maybe<bool> Boolean(string key) => String(key).Map(Maybe.Boolean);

      public Maybe<DateTime> DateTime(string key) => String(key).Map(Maybe.DateTime);

      public Maybe<Guid> Guid(string key) => String(key).Map(Maybe.Guid);

      public Maybe<FileName> FileName(string key)
      {
         try
         {
            var _file = String(key).Map(s => (FileName)s);
            return _file.Map(f => f.IsValid) | false ? _file : nil;
         }
         catch
         {
            return nil;
         }
      }

      public Maybe<FolderName> FolderName(string key)
      {
         try
         {
            var _folder = String(key).Map(s => (FolderName)s);
            return _folder.Map(f=>f.IsValid) | false ? _folder : nil;
         }
         catch
         {
            return nil;
         }
      }

      public Maybe<byte[]> Bytes(string key)
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

      public Maybe<TimeSpan> TimeSpan(string key) => String(key).Map(Maybe.TimeSpan);

      public Maybe<string[]> Strings(string key) => String(key).Map(s => s.Unjoin("/s* ',' /s*"));
   }
}