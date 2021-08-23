using System.Collections.Generic;
using System.Linq;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Consoles
{
   public class CommandLineHistory
   {
      protected List<string> lines;
      protected int position;

      public CommandLineHistory()
      {
         lines = new List<string>();
         position = 0;
      }

      public void Add(string line)
      {
         if (line.IsNotEmpty())
         {
            if (lines.Count > 0)
            {
               if (lines.Last() != line)
               {
                  lines.Add(line);
               }
               else
               {
                  lines.Add(line);
               }
            }
         }

         position = lines.Count;
      }

      public Maybe<string> Current => maybe(position.Between(0).Until(lines.Count), () => lines[position]);

      public Maybe<string> Forward()
      {
         if (position + 1 < lines.Count)
         {
            position++;
            return Current;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<string> Backward()
      {
         if (position > 0)
         {
            position--;
            return Current;
         }
         else
         {
            return nil;
         }
      }
   }
}