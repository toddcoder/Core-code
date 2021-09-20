using System;
using Core.Services.Plugins;

namespace Standard.Services.Plugins
{
	public class PluginException : ApplicationException
	{
		public PluginException(Plugin plugin, Exception innerException)
			: base($"Plugin: {plugin.Name}; {innerException.Message}")
		{
		}
	}
}