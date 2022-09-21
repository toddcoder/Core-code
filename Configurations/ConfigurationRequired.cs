using System;
using Core.Computers;

namespace Core.Configurations
{
   public class ConfigurationRequired
   {
      protected ConfigurationItem configurationItem;

      internal ConfigurationRequired(ConfigurationItem configurationItem)
      {
         this.configurationItem = configurationItem;
      }

      public Setting Setting(string key) => configurationItem.Result.Setting(key);

      public Item Item(string key) => configurationItem.Result.Item(key);

      public string String(string key) => configurationItem.Result.String(key);

      public int Int32(string key) => configurationItem.Result.Int32(key);

      public long Int64(string key) => configurationItem.Result.Int64(key);

      public float Single(string key) => configurationItem.Result.Single(key);

      public double Double(string key) => configurationItem.Result.Double(key);

      public bool Boolean(string key) => configurationItem.Result.Boolean(key).ForceValue();

      public DateTime DateTime(string key) => configurationItem.Result.DateTime(key);

      public Guid Guid(string key) => configurationItem.Result.Guid(key);

      public FileName FileName(string key) => configurationItem.Result.FileName(key);

      public FolderName FolderName(string key) => configurationItem.Result.FolderName(key);

      public byte[] Bytes(string key) => configurationItem.Result.Bytes(key);

      public TimeSpan TimeSpan(string key) => configurationItem.Result.TimeSpan(key);

      public string[] Strings(string key) => configurationItem.Result.Strings(key);
   }
}