using System.Collections;
using System.Collections.Generic;
using Core.Enumerables;

namespace Core.Strings
{
   public class ListString : IEnumerable<string>
   {
      public static implicit operator string(ListString listString) => listString.Text;

      protected List<string> list;
      protected string separator;
      protected bool unique;

      public ListString(string initialString, string separator = ", ", bool unique = false)
      {
         list = new List<string> { initialString };
         this.separator = separator;
         this.unique = unique;
      }

      public string Separator
      {
         get => separator;
         set => separator = value;
      }

      public bool Unique
      {
         get => unique;
         set => unique = value;
      }

      public string Text
      {
         get => list.ToString(separator);
         set
         {
            var contains = list.Contains(value);
            if (contains && !unique || !contains)
            {
               list.Add(value);
            }
         }
      }

      public IEnumerator<string> GetEnumerator() => list.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public override string ToString() => Text;
   }
}