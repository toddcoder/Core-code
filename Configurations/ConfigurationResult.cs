using System;
using Core.Computers;
using Core.Monads;
using Core.Strings;
using static Core.Objects.ConversionFunctions;

namespace Core.Configurations
{
   public class ConfigurationResult
   {
      protected IConfigurationItemGetter getter;

      internal ConfigurationResult(IConfigurationItemGetter getter)
      {
         this.getter = getter;
      }

      public Result<Setting> Setting(string key) => getter.GetSetting(key).Required($"Setting {key} required");

      public Result<Item> Item(string key) => getter.GetItem(key).Required($"Item {key} required");

      public Result<string> String(string key) => getter.GetItem(key).Map(i => i.Text).Required($"Item {key} required");

      public Result<int> Int32(string key) => String(key).Map(Result.Int32);

      public Result<long> Int64(string key) => String(key).Map(Result.Int64);

      public Result<float> Single(string key) => String(key).Map(Result.Single);

      public Result<double> Double(string key) => String(key).Map(Result.Double);

      public Result<bool> Boolean(string key) => String(key).Map(Result.Boolean);

      public Result<DateTime> DateTime(string key) => String(key).Map(Result.DateTime);

      public Result<Guid> Guid(string key) => String(key).Map(Result.Guid);

      public Result<FileName> FileName(string key)
      {
         try
         {
            var _file = String(key).Map(s => (FileName)s);
            return _file.Map(f => f.IsValid) | false ? _file : (Exception)_file;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Result<FolderName> FolderName(string key)
      {
         try
         {
            var _folder = String(key).Map(s => (FolderName)s);
            return _folder.Map(f => f.IsValid) | false ? _folder : (Exception)_folder;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Result<byte[]> Bytes(string key)
      {
         try
         {
            return String(key).Map(s => s.FromBase64());
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Result<TimeSpan> TimeSpan(string key) => String(key).Map(Result.TimeSpan);
   }
}