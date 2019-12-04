using System.Collections.Generic;
using System.IO;

namespace Core.Strings.Text
{
   public class DifferenceModel
   {
      public DifferenceModel()
      {
         OldDiffItems = new List<DifferenceItem>();
         NewDiffItems = new List<DifferenceItem>();
      }

      public List<DifferenceItem> OldDiffItems { get; }

      public List<DifferenceItem> NewDiffItems { get; }

      public override string ToString()
      {
         using (var writer = new StringWriter())
         {
            writer.WriteLine("old");
            foreach (var item in OldDiffItems)
            {
               writer.WriteLine(item);
            }
            writer.WriteLine("new");
            foreach (var item in NewDiffItems)
            {
               writer.WriteLine(item);
            }

            return writer.ToString();
         }
      }
   }
}