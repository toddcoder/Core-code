using System.Collections.Generic;

namespace Core.Strings.Text
{
   public class TextDiffResult
   {
      public TextDiffResult(string[] oldItems, string[] newItems, List<TextDiffBlock> textDiffBlocks)
      {
         OldItems = oldItems;
         NewItems = newItems;
         TextDiffBlocks = textDiffBlocks;
      }
      public string[] OldItems { get; }

      public string[] NewItems { get; }

      public List<TextDiffBlock> TextDiffBlocks { get; }
   }
}