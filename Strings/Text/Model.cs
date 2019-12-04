using System.Collections.Generic;
using System.IO;

namespace Core.Strings.Text
{
   public class Model
   {
      public Model()
      {
         OldDiffItems = new List<DiffItem>();
         NewDiffItems = new List<DiffItem>();
      }

      public List<DiffItem> OldDiffItems { get; }

      public List<DiffItem> NewDiffItems { get; }

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