using Core.Monads;
using Core.Strings;

namespace Core.Configurations
{
   public static class ConfigurationExtensions
   {
      public static Maybe<int> GetInt32(this IConfigurationItem item, string key) => item.GetValue(key).Map(s => s.AsInt());

      public static Maybe<double> GetDouble(this IConfigurationItem item, string key) => item.GetValue(key).Map(s => s.AsDouble());

      public static Maybe<bool> GetBoolean(this IConfigurationItem item, string key) => item.GetValue(key).Map(s => s.AsBool());

      public static Maybe<byte[]> GetBytes(this IConfigurationItem item, string key) => item.GetValue(key).Map(s => s.FromBase64());

      public static Result<int> RequireInt32(this IConfigurationItem item, string key) => item.RequireValue(key).Map(s => s.Int32());

      public static Result<double> RequireDouble(this IConfigurationItem item, string key) => item.RequireValue(key).Map(s => s.Double());

      public static Result<bool> RequireBoolean(this IConfigurationItem item, string key) => item.RequireValue(key).Map(s => s.Boolean());

      public static Result<byte[]> RequireBytes(this IConfigurationItem item, string key) => item.RequireValue(key).Map(s => s.FromBase64());

      public static int Int32At(this IConfigurationItem item, string key, int defaultValue = default)
      {
         return item.ValueAt(key).ToInt(defaultValue);
      }

      public static double DoubleAt(this IConfigurationItem item, string key, double defaultValue = default)
      {
         return item.ValueAt(key).ToDouble(defaultValue);
      }

      public static bool BooleanAt(this IConfigurationItem item, string key, bool defaultValue = default)
      {
         return item.ValueAt(key).ToBool(defaultValue);
      }

      public static byte[] BytesAt(this IConfigurationItem item, string key)
      {
         return item.ValueAt(key).FromBase64();
      }
   }
}