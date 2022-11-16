using System.Collections.Generic;
using Core.Monads;

namespace Core.Configurations;

public interface IConfigurationItem
{
   string Key { get; }

   string this[string key] { get; set; }

   IConfigurationItem GetItem(string key);

   Maybe<IConfigurationItem> GetSomeItem(string key);

   void SetItem(string key, IConfigurationItem item);

   Maybe<string> GetValue(string key);

   Result<string> RequireValue(string key);

   string ValueAt(string key);

   public IEnumerable<(string key, string value)> Values();

   string At(string key);

   Maybe<Setting> GetSetting(string key);

   Result<Setting> RequireSetting(string key);

   Setting SettingAt(string key);

   public IEnumerable<(string key, Setting setting)> Settings();

   public int Count { get; }
}