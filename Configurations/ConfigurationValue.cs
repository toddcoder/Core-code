using System;
using Core.Computers;
using Core.Strings;
using static Core.Objects.ConversionFunctions;

namespace Core.Configurations
{
   public class ConfigurationValue
   {
      protected IConfigurationItemGetter getter;

      internal ConfigurationValue(IConfigurationItemGetter getter)
      {
         this.getter = getter;
      }

      public Setting Setting(string key) => getter.GetSetting(key) | (() => new Setting());

      public Item Item(string key) => getter.GetItem(key) | (() => new Item("", ""));

      public string String(string key) => getter.GetItem(key).Map(i => i.Text) | "";

      public int Int32(string key) => Value.Int32(String(key));

      public long Int64(string key) => Value.Int64(String(key));

      public float Single(string key) => Value.Single(String(key));

      public double Double(string key) => Value.Double(String(key));

      public bool Boolean(string key) => Value.Boolean(String(key));

      public DateTime DateTime(string key) => Value.DateTime(String(key));

      public Guid Guid(string key) => Value.Guid(String(key));

      public FileName FileName(string key) => String(key);

      public FolderName FolderName(string key) => String(key);

      public byte[] Bytes(string key) => String(key).FromBase64();

      public TimeSpan TimeSpan(string key) => Value.TimeSpan(String(key));
   }
}