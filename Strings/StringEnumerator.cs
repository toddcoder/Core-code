using System.Collections;
using System.Collections.Generic;

namespace Core.Strings
{
   public class StringEnumerator : IEnumerator<string>
   {
      string source;
      int index;

      internal StringEnumerator(string source)
      {
         this.source = source;
         index = -1;
      }

      public void Dispose() { }

      public bool MoveNext() => ++index < source.Length;

      public void Reset() => index = -1;

      public string Current => source.Substring(index, 1);

      object IEnumerator.Current => Current;
   }
}