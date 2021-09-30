using Core.Monads;
using Core.Strings;
using static Core.Objects.ConversionFunctions;

namespace Core.Configurations
{
   public static class ConfigurationExtensions
   {
      public static Maybe<int> GetInt32(this IConfigurationItem item, string key) => item.GetValue(key).Map(Maybe.Int32);

      public static Maybe<double> GetDouble(this IConfigurationItem item, string key) => item.GetValue(key).Map(Maybe.Double);

      public static Maybe<bool> GetBoolean(this IConfigurationItem item, string key) => item.GetValue(key).Map(Maybe.Boolean);

      public static Maybe<byte[]> GetBytes(this IConfigurationItem item, string key) => item.GetValue(key).Map(s => s.FromBase64());

      public static Result<int> RequireInt32(this IConfigurationItem item, string key) => item.RequireValue(key).Map(Result.Int32);

      public static Result<double> RequireDouble(this IConfigurationItem item, string key) => item.RequireValue(key).Map(Result.Double);

      public static Result<bool> RequireBoolean(this IConfigurationItem item, string key) => item.RequireValue(key).Map(Result.Boolean);

      public static Result<byte[]> RequireBytes(this IConfigurationItem item, string key) => item.RequireValue(key).Map(s => s.FromBase64());

      public static int Int32At(this IConfigurationItem item, string key, int defaultValue = default)
      {
         return Value.Int32(item.ValueAt(key), defaultValue);
      }

      public static double DoubleAt(this IConfigurationItem item, string key, double defaultValue = default)
      {
         return Value.Double(item.ValueAt(key), defaultValue);
      }

      public static bool BooleanAt(this IConfigurationItem item, string key, bool defaultValue = default)
      {
         return Value.Boolean(item.ValueAt(key), defaultValue);
      }

      public static byte[] BytesAt(this IConfigurationItem item, string key)
      {
         return item.ValueAt(key).FromBase64();
      }
   }
}