using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Strings
{
   public class Slicer
   {
      class Replacement
      {
         public Replacement(int index, int length, string text)
         {
            Index = index;
            Length = length;
            Text = text.ToNonNullString();
         }

         public int Index { get; }

         public int Length { get; }

         public string Text { get; }
      }

      public static implicit operator Slicer(string text) => new Slicer(text);

      string text;
      List<Replacement> replacements;

      public Slicer(string text)
      {
         this.text = text;
         replacements = new List<Replacement>();
      }

      public bool IsEmpty => text.Length == 0;

      public string this[int index, int length]
      {
         get => text.Drop(index).Keep(length);
         set
         {
            if (!IsEmpty)
            {
               replacements.Add(new Replacement(index, length, value));
            }
         }
      }

      public char this[int index]
      {
         get
         {
            var item = this[index, 1];
            return item.Length > 0 ? item[0] : (char)0;
         }
         set => this[index, 1] = value.ToString();
      }

      public int Length => text.Length;

      public override string ToString()
      {
         var offset = 0;
         var builder = new StringBuilder(text);

         foreach (var replacement in replacements.OrderBy(r => r.Index))
         {
            var newIndex = replacement.Index + offset;
            var length = replacement.Length;
            if (newIndex >= 0)
            {
               if (length + newIndex > builder.Length)
               {
                  length = builder.Length - newIndex;
               }

               builder.Remove(newIndex, length);
               if (replacement.Text.IsNotEmpty())
               {
                  builder.Insert(newIndex, replacement.Text);
                  offset += replacement.Text.Length - length;
               }
               else
               {
                  offset -= length;
               }
            }
         }

         return builder.ToString();
      }

      public void Reset() => replacements.Clear();
   }
}