using System;
using Core.Collections;
using Core.Computers;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Settings
{
   public class SettingMaybe
   {
      protected Setting setting;

      internal SettingMaybe(Setting setting)
      {
         this.setting = setting;
      }

      public Maybe<string> Text => setting.Value.Maybe.Left;

      public Maybe<StringHash<Setting>> SubSettings => setting.Value.Maybe.Right;

      public Maybe<Setting> Setting(string key) => SubSettings.Map(ss => ss.Map(key));

      public Maybe<int> Int32() => Text.Map(Maybe.Int32);

      public Maybe<int> Int32(string key) => Setting(key).Map(s => s.Maybe.Int32());

      public Maybe<long> Int64() => Text.Map(Maybe.Int64);

      public Maybe<long> Int64(string key) => Setting(key).Map(s => s.Maybe.Int64());

      public Maybe<float> Single() => Text.Map(Maybe.Single);

      public Maybe<float> Single(string key) => Setting(key).Map(s => s.Maybe.Single());

      public Maybe<double> Double() => Text.Map(Maybe.Double);

      public Maybe<double> Double(string key) => Setting(key).Map(s => s.Maybe.Double());

      public Maybe<bool> Boolean() => Text.Map(Maybe.Boolean);

      public Maybe<bool> Boolean(string key) => Setting(key).Map(s => s.Maybe.Boolean());

      public Maybe<DateTime> DateTime() => Text.Map(Maybe.DateTime);

      public Maybe<DateTime> DateTime(string key) => Setting(key).Map(s => s.Maybe.DateTime());

      public Maybe<Guid> Guid() => Text.Map(Maybe.Guid);

      public Maybe<Guid> Guid(string key) => Setting(key).Map(s => s.Maybe.Guid());

      public Maybe<FileName> FileName()
      {
         try
         {
            if (Text.Map(out var text))
            {
               FileName file = text;
               return maybe<FileName>() & file.IsValid & file;
            }
            else
            {
               return nil;
            }
         }
         catch
         {
            return nil;
         }
      }

      public Maybe<FileName> FileName(string key) => Setting(key).Map(s => s.Maybe.FileName());

      public Maybe<FolderName> FolderName()
      {
         try
         {
            if (Text.Map(out var text))
            {
               FolderName folder = text;
               return maybe<FolderName>() & folder.IsValid & folder;
            }
            else
            {
               return nil;
            }
         }
         catch
         {
            return nil;
         }
      }

      public Maybe<FolderName> FolderName(string key) => Setting(key).Map(s => s.Maybe.FolderName());

      public Maybe<byte[]> Bytes()
      {
         try
         {
            if (Text.Map(out var text))
            {
               return text.FromBase64();
            }
            else
            {
               return nil;
            }
         }
         catch
         {
            return nil;
         }
      }

      public Maybe<byte[]> Bytes(string key) => Setting(key).Map(s => s.Maybe.Bytes());

      public Maybe<string[]> Array()
      {
         if (setting.IsArray && Text.Map(out var text))
         {
            return text.Unjoin(@"/s* ',' /s*");
         }
         else
         {
            return nil;
         }
      }

      public Maybe<string> String(string key) => Setting(key).Map(s => s.Maybe.Text);
   }
}