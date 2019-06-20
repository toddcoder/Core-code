using System;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Configurations
{
	public class ConfigurationNode
	{
		protected string name;
		protected IMaybe<object> value;
		protected Lazy<Hash<string, ConfigurationNode>> children;
		protected IMaybe<Type> type;

		public ConfigurationNode(string name, object value = null)
		{
			this.name = name;
			this.value = value.SomeIfNotNull();
			type = this.value.Map(v => v.GetType());
			children = new Lazy<Hash<string, ConfigurationNode>>(() => new Hash<string, ConfigurationNode>());
		}

		public string Name => name;

		public IMaybe<object> Value => value;

		public IMaybe<Type> Type => type;

		public IMaybe<ConfigurationNode> this[string childName]
		{
			get
			{
				if (value.IsSome)
            {
               return none<ConfigurationNode>();
            }
            else
            {
               return children.Value.Map(childName, cn => cn);
            }
         }
			set
			{
				if (value.If(out var configurationNode))
            {
               children.Value[childName] = configurationNode;
            }
            else
            {
               children.Value.Remove(childName);
            }
         }
		}
	}
}