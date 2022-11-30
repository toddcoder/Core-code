using System.Collections.Generic;

namespace Core.Strings.Text;

public class DifferenceResult
{
   public DifferenceResult(string[] oldItems, string[] newItems, List<DifferenceBlock> differenceBlocks)
   {
      OldItems = oldItems;
      NewItems = newItems;
      DifferenceBlocks = differenceBlocks;
   }

   public string[] OldItems { get; }

   public string[] NewItems { get; }

   public List<DifferenceBlock> DifferenceBlocks { get; }
}