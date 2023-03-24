using Core.Monads;
using Core.Strings;
using static Core.Objects.ConversionFunctions;

namespace Core.Configurations;

public static class ConfigurationExtensions
{
   public static Optional<int> GetInt32(this IConfigurationItem item, string key) => item.GetValue(key).Map(i => Maybe.Int32(i));

   public static Optional<double> GetDouble(this IConfigurationItem item, string key) => item.GetValue(key).Map(d => Maybe.Double(d));

   public static Optional<bool> GetBoolean(this IConfigurationItem item, string key) => item.GetValue(key).Map(Maybe.Boolean);

   public static Optional<byte[]> GetBytes(this IConfigurationItem item, string key) => item.GetValue(key).Map(s => s.FromBase64());

   public static Optional<int> RequireInt32(this IConfigurationItem item, string key) => item.RequireValue(key).Map(i => Result.Int32(i));

   public static Optional<double> RequireDouble(this IConfigurationItem item, string key) => item.RequireValue(key).Map(d => Result.Double(d));

   public static Optional<bool> RequireBoolean(this IConfigurationItem item, string key) => item.RequireValue(key).Map(Result.Boolean);

   public static Optional<byte[]> RequireBytes(this IConfigurationItem item, string key) => item.RequireValue(key).Map(s => s.FromBase64());

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