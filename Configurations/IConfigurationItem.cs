using Core.Monads;

namespace Core.Configurations
{
   public interface IConfigurationItem
   {
      string Key { get; }

      IConfigurationItem this[string key] { get; }

      Maybe<string> GetValue(string key);

      IResult<string> RequireValue(string key);

      Maybe<Group> GetGroup(string key);

      IResult<Group> RequireGroup(string key);
   }
}