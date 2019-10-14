using Core.Collections;
using Core.Monads;

namespace Core.Configurations
{
	public class Configuration
	{
		protected Hash<string, ConfigurationNode> children;

		public Configuration() => children = new Hash<string, ConfigurationNode>();

		public Configuration(string source)
		{
		}

		public IMaybe<ConfigurationNode> this[string childName]
		{
			get => children.Map(childName, cn => cn);
			set
			{
				if (value.If(out var configurationNode))
            {
               children[childName] = configurationNode;
            }
            else
            {
               children.Remove(childName);
            }
         }
		}
	}
}