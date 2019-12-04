using System.Collections.Generic;

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
   }
}