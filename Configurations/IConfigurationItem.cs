using Core.Monads;

namespace Core.Configurations
{
   public interface IConfigurationItem
   {
      string Key { get; }

      IConfigurationItem this[string key] { get; }

      IMaybe<string> GetValue(string key);

      IResult<string> RequireValue(string key);

      IMaybe<Group> GetGroup(string key);

      IResult<Group> RequireGroup(string key);
   }
}