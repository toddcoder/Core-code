using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Strings;

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

   public static implicit operator Slicer(string text) => new(text);

   protected string text;
   protected List<Replacement> replacements;
   protected int offset;
   protected int currentLength;

   public Slicer(string text)
   {
      this.text = text;
      replacements = new List<Replacement>();
      offset = 0;
      currentLength = 0;
   }

   public bool IsEmpty => text.Length == 0;

   protected void updateOffsets(int index, int length, string replacementText)
   {
      offset = index + offset;
      var replacementLength = length;
      if (offset >= 0)
      {
         if (replacementLength + offset > currentLength)
         {
            replacementLength = currentLength - offset;
         }

         currentLength -= replacementLength;
         if (replacementText.IsNotEmpty())
         {
            currentLength += replacementText.Length;
            offset += replacementText.Length - replacementLength;
         }
         else
         {
            offset -= replacementLength;
         }
      }

      OffsetIndex = offset;
      OffsetLength = replacementLength;
   }

   public string this[int index, int length]
   {
      get => text.Drop(index).Keep(length);
      set
      {
         if (!IsEmpty)
         {
            replacements.Add(new Replacement(index, length, value));
            updateOffsets(index, length, value);
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

   public void Keep(int index, int length)
   {
      replacements.Add(new Replacement(0, index, ""));
      replacements.Add(new Replacement(length, -1, ""));
   }

   public string Text => text;

   public int Length => text.Length;

   public IEnumerator<(int index, int length, string text)> GetEnumerator()
   {
      offset = 0;
      currentLength = text.Length;

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
               offset -= replacementLength;
            }

            yield return (offsetIndex, length, replacementText);
         }
      }
   }

   public IEnumerable<Replacement> Replacements => replacements.OrderBy(r => r.Index);

   public override string ToString()
   {
      offset = 0;
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

         OffsetIndex = offsetIndex;
         OffsetLength = replacementLength;
      }

      return builder.ToString();
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Reset()
   {
      replacements.Clear();
      OffsetIndex = 0;
      OffsetLength = 0;
   }

   public int OffsetIndex { get; set; }

   public int OffsetLength { get; set; }
}