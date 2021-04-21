using Core.Collections;
using Core.Monads;

namespace Core.Configurations
{
   public class Configuration : IHash<string, IConfigurationItem>
   {
      protected Group root;

      internal Configuration(Group root)
      {
         this.root = root;
      }

      public IConfigurationItem this[string key] => root[key];

      public bool ContainsKey(string key) => root.ContainsKey(key);

      public IResult<Hash<string, IConfigurationItem>> AnyHash() => root.AnyHash();
   }
}