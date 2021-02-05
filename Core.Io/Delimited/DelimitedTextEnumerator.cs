using System.Collections;
using System.Collections.Generic;
using Core.Assertions;
using static Core.Assertions.AssertionFunctions;

namespace Core.Io.Delimited
{
   public class DelimitedTextEnumerator : IEnumerator<string[]>
   {
      protected DelimitedTextReader reader;
      protected string[] current;

      public DelimitedTextEnumerator(DelimitedTextReader reader)
      {
         this.reader = assert(() => (object)reader).Must().Not.BeNull().Force<DelimitedTextReader>();
         current = new string[reader.FieldCount];
      }

      public void Dispose() => reader.Dispose();

      public bool MoveNext()
      {
         if (reader.ReadRecord())
         {
            reader.CopyFields(current);

            return true;
         }
         else
         {
            return false;
         }
      }

      public void Reset()
      {
      }

      public string[] Current => current;

      object IEnumerator.Current => Current;
   }
}