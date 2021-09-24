using System.Collections.Generic;
using Core.Monads;

namespace Core.Configurations
{
   public interface IConfigurationItem
   {
      string Key { get; }

      IConfigurationItem this[string key] { get; }

      Maybe<string> GetValue(string key);

      Result<string> RequireValue(string key);

      string ValueAt(string key);

      public IEnumerable<(string key, string value)> Values();

      string At(string key);

      Maybe<Group> GetGroup(string key);

      Result<Group> RequireGroup(string key);

      Group GroupAt(string key);

      public IEnumerable<(string key, Group group)> Groups();

      public int Count { get; }
   }
}