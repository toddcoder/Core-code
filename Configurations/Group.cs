using Core.Collections;
using Core.Monads;

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

      public IMaybe<IConfigurationItem> Map(string key) => items.Map(key);

      public IMaybe<string> GetValue(string key) => items.Map(key).Map(i => i.GetValue(key));

      public bool ContainsKey(string key) => items.ContainsKey(key);

      public IResult<Hash<string, IConfigurationItem>> AnyHash() => items.AsHash.Success();
   }
}