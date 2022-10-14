using System;

namespace Core.Services.Plugins;

public class PluginException : ApplicationException
{
   public PluginException(Plugin plugin, Exception innerException)
      : base($"Plugin: {plugin.Name}; {innerException.Message}")
   {
   }
}