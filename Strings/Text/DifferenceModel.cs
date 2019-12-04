using System.Collections.Generic;
using System.IO;

namespace Core.Strings.Text
{
   public class DifferenceModel
   {
      public DifferenceModel()
      {
         OldDifferenceItems = new List<DifferenceItem>();
         NewDifferenceItems = new List<DifferenceItem>();
      }

      public List<DifferenceItem> OldDifferenceItems { get; }

      public List<DifferenceItem> NewDifferenceItems { get; }

      public override string ToString()
      {
         using (var writer = new StringWriter())
         {
            writer.WriteLine("old:");
            foreach (var item in OldDifferenceItems)
            {
               writer.WriteLine(item);
            }

            writer.WriteLine();

            writer.WriteLine("new:");
            foreach (var item in NewDifferenceItems)
            {
               writer.WriteLine(item);
            }

            return writer.ToString();
         }
      }
   }
}