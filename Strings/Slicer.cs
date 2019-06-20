using System.Text;

namespace Core.Strings
{
   public class Slicer
   {
      public static implicit operator Slicer(string text) => new Slicer(text);

      StringBuilder text;
      int offset;

      public Slicer(string text)
      {
         this.text = new StringBuilder(text);
         offset = 0;
      }

      public bool IsEmpty => text.Length == 0;

      public string this[int index, int length]
      {
         get
         {
            if (!IsEmpty)
            {
               var newIndex = index + offset;
               if (newIndex < 0 || newIndex > text.Length + 1)
               {
                  return "";
               }
               else if (newIndex + length < 0 || newIndex + length > text.Length)
               {
                  return "";
               }
               else
               {
                  return text.ToString(newIndex, length);
               }
            }
            else
            {
               return "";
            }
         }
         set
         {
            if (!IsEmpty)
            {
               var newIndex = index + offset;
               if (newIndex >= 0)
               {
                  if (length + newIndex > text.Length)
                  {
                     length = text.Length - newIndex;
                  }

                  text.Remove(newIndex, length);
                  if (value.IsNotEmpty())
                  {
                     text.Insert(newIndex, value);
                     offset += value.Length - length;
                  }
                  else
                  {
                     offset -= length;
                  }
               }
            }
         }
      }

      public string this[int index]
      {
         get => this[index, 1];
         set => this[index, 1] = value;
      }

      public char CharAt(int index)
      {
         var newIndex = index + offset;
         if (newIndex < 0 || newIndex > text.Length + 1)
         {
            return (char)0;
         }
         else
         {
            return text[newIndex];
         }
      }

      public int Length => text.Length;

      public string Substring(int index)
      {
         offset = 0;
         return text.ToString(index, text.Length - index);
      }

      public void SetSubstring(int index, string value)
      {
         offset = 0;
         var length = text.Length;

         this[index, length - index] = value;
      }

      public override string ToString() => text.ToString();

      public void Reset() => offset = 0;
   }
}