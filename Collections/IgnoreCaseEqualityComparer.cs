using System.Collections.Generic;
using Core.Strings;

namespace Core.Collections
{
   public class IgnoreCaseEqualityComparer : IEqualityComparer<string>
   {
      public bool Equals(string x, string y) => x.ToNonNullString().Same(y.ToNonNullString());

      public int GetHashCode(string obj) => obj.ToUpper().GetHashCode();
   }
}