using System;

namespace Core.Collections;

public static class CollectionFunctions
{
   public static StringComparer stringComparer(bool ignoreCase)
   {
      return ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
   }

   public static StringComparison stringComparison(bool ignoreCase)
   {
      return ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
   }
}