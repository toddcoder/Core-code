using System;
using System.Collections.Generic;

namespace Core.Collections;

public class UserStringComparer : IComparer<string>, IEqualityComparer<string>
{
   protected Func<string, string, int> comparer;
   protected Func<string, string, bool> equals;

   public UserStringComparer(Func<string, string, int> comparer, Func<string, string, bool> equals)
   {
      this.comparer = comparer;
      this.equals = equals;
   }

   public int Compare(string x, string y) => comparer(x, y);

   public bool Equals(string x, string y) => equals(x, y);

   public int GetHashCode(string obj) => obj is null ? throw new ArgumentNullException(nameof(obj)) : GetHashCode(obj);
}