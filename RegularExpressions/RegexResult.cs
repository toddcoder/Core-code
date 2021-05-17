using System;

namespace Core.RegularExpressions
{
   public class RegexResult
   {
      public RegexResult(string text, int index, int length, string[] groups, int itemIndex)
      {
         Text = text;
         Index = index;
         Length = length;
         Groups = groups;
         ItemIndex = itemIndex;

         IsFound = true;
      }

      public RegexResult()
      {
         Text = "";
         Index = -1;
         Length = -1;
         Groups = Array.Empty<string>();
         ItemIndex = -1;
         IsFound = false;
      }

      public string Text { get; }

      public int Index { get; }

      public int Length { get; }

      public string[] Groups { get; }

      public int ItemIndex { get; }

      public bool IsFound { get; }

      public void Deconstruct(out string text, out int index, out int length, out string[] groups, out int itemIndex, out bool isFound)
      {
         text = Text;
         index = Index;
         length = Length;
         groups = Groups;
         itemIndex = ItemIndex;
         isFound = IsFound;
      }
   }
}