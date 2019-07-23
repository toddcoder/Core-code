using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Strings
{
   public class Slicer : IEnumerable<(int index, int length, string text)>
   {
      public class Replacement
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

         public override string ToString() => $"\"{Text.Truncate(80)}\"[{Index}, {Length}]";

         public void Deconstruct(out int index, out int length, out string text)
         {
            index = Index;
            length = Length;
            text = Text;
         }
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

      public IEnumerator<(int index, int length, string text)> GetEnumerator()
      {
         var offset = 0;
         var currentLength = text.Length;

         foreach (var (index, length, replacementText) in replacements.OrderBy(r => r.Index))
         {
            var offsetIndex = index + offset;
            var replacementLength = length;
            if (offsetIndex >= 0)
            {
               if (replacementLength + offsetIndex > currentLength)
               {
                  replacementLength = currentLength - offsetIndex;
               }

               currentLength -= replacementLength;
               if (replacementText.IsNotEmpty())
               {
                  currentLength += replacementText.Length;
                  offset += replacementText.Length - replacementLength;
               }
               else
               {
                  offset += replacementText.Length - replacementLength;
               }

               yield return (offsetIndex, length, replacementText);
            }
         }
      }

      public IEnumerable<Replacement> Replacements => replacements.OrderBy(r => r.Index);

      public override string ToString()
      {
         var offset = 0;
         var builder = new StringBuilder(text);

         foreach (var (index, length, replacementText) in replacements.OrderBy(r => r.Index))
         {
            var offsetIndex = index + offset;
            var replacementLength = length;
            if (offsetIndex >= 0)
            {
               if (replacementLength + offsetIndex > builder.Length)
               {
                  replacementLength = builder.Length - offsetIndex;
               }

               builder.Remove(offsetIndex, replacementLength);
               if (replacementText.IsNotEmpty())
               {
                  builder.Insert(offsetIndex, replacementText);
                  offset += replacementText.Length - replacementLength;
               }
               else
               {
                  offset -= replacementLength;
               }
            }
         }

         return builder.ToString();
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public void Reset() => replacements.Clear();
   }
}