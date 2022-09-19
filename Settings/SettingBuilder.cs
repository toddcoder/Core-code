using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Settings
{
   public class SettingBuilder
   {
      protected string key;
      protected Maybe<string> _text;
      protected StringHash<SettingBuilder> subSettings;

      public SettingBuilder(string key)
      {
         this.key = key;

         _text = nil;
         subSettings = new StringHash<SettingBuilder>(true);
      }

      public string Key => key;

      public void SetText(string text)
      {
         _text = text;
         subSettings.Clear();
      }

      public void SetSubSetting(string key, SettingBuilder builder)
      {
         _text = nil;
         subSettings[key] = builder;
      }

      public bool IsArray { get; set; }

      public Setting Setting()
      {
         if (_text.Map(out var text))
         {
            return new Setting(key, text) { IsArray = IsArray };
         }
         else
         {
            var newSubSettings = new StringHash<Setting>(true);
            foreach (var (settingKey, builder) in subSettings)
            {
               newSubSettings[settingKey] = builder.Setting();
            }

            return new Setting(key, newSubSettings);
         }
      }
   }
}