using Core.Monads;

namespace Core.Configurations
{
   public interface IConfigurationItem
   {
      string Key { get; }

      IConfigurationItem this[string key] { get; }

      IMaybe<IConfigurationItem> Map(string key);

      IMaybe<string> GetValue(string key);
   }
}