using Core.Monads;

namespace Core.Configurations
{
   public interface IConfigurationItem
   {
      string Key { get; }

      IConfigurationItem this[string key] { get; }

      Maybe<string> GetValue(string key);

      Result<string> RequireValue(string key);

      Maybe<Group> GetGroup(string key);

      Result<Group> RequireGroup(string key);
   }
}