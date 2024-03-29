﻿using System;
using System.Linq;
using Core.Computers;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Configurations;

public class ConfigurationResult
{
   protected IConfigurationItemGetter getter;

   internal ConfigurationResult(IConfigurationItemGetter getter)
   {
      this.getter = getter;
   }

   public Result<Setting> Setting(string key) => getter.GetSetting(key).Result($"Setting {key} required");

   public Result<Item> Item(string key) => getter.GetItem(key).Result($"Item {key} required");

   public Result<string> String(string key) => getter.GetItem(key).Map(i => i.Text).Result($"Item {key} required");

   public Result<int> Int32(string key) => String(key).Map(i => Result.Int32(i));

   public Result<long> Int64(string key) => String(key).Map(l => Result.Int64(l));

   public Result<float> Single(string key) => String(key).Map(s => Result.Single(s));

   public Result<double> Double(string key) => String(key).Map(d => Result.Double(d));

   public Result<bool> Boolean(string key) => String(key).Map(Result.Boolean);

   public Result<DateTime> DateTime(string key) => String(key).Map(Result.DateTime);

   public Result<Guid> Guid(string key) => String(key).Map(Result.Guid);

   public Result<FileName> FileName(string key)
   {
      try
      {
         var _fileName = String(key);
         if (_fileName is (true, var fileName))
         {
            if (fileName.IsEmpty())
            {
               return fail("File name is empty");
            }
         }
         else
         {
            return _fileName.Exception;
         }

         var _file = _fileName.Map(s => (FileName)s);
         return _file.Map(f => f.IsValid) | false ? _file : _file.Exception;
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
         var _folderName = String(key);
         if (_folderName is (true, var folderName))
         {
            if (folderName.IsEmpty())
            {
               return fail("Folder name is empty");
            }
         }
         else
         {
            return _folderName.Exception;
         }

         var _folder = _folderName.Map(s => (FolderName)s);
         return _folder.Map(f => f.IsValid) | false ? _folder : _folder.Exception;
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

   public Result<string[]> Strings(string key) => String(key).Map(s => s.Unjoin("/s* ',' /s*"));

   public Result<string[]> SettingTexts(string key) => Setting(key).Map(s => s.Items().Select(i => i.text).ToArray());

   public Result<string[]> SettingKeys(string key) => Setting(key).Map(s => s.Items().Select(i => i.key).ToArray());
}