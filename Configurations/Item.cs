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

      public IConfigurationItem this[string key] => this;

      public IMaybe<IConfigurationItem> Map(string key) => maybe(key.Same(Key), () => (IConfigurationItem)this);

      public IMaybe<string> GetValue(string key) => maybe(key.Same(Key), () => Value);

      public string Value { get; }

      public override string ToString() => $"{Key}: \"{Value}\"";
   }
}