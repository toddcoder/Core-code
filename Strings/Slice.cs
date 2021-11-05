using System;

namespace Core.Strings
{
   public struct Slice : IEquatable<Slice>
   {
      public string Text;
      public int Index;
      public int Length;

      public Slice(string text, int index, int length): this()
      {
         Index = index;
         Length = length;
         Text = text;
      }

      public void Deconstruct(out string text, out int index, out int length)
      {
         text = Text;
         index = Index;
         length = Length;
      }

      public bool Equals(Slice other) => Text == other.Text && Index == other.Index && Length == other.Length;

      public override bool Equals(object obj) => obj is Slice other && Equals(other);

      public override int GetHashCode()
      {
         unchecked
         {
            var hashCode = Text != null ? Text.GetHashCode() : 0;
            hashCode = hashCode * 397 ^ Index;
            hashCode = hashCode * 397 ^ Length;
            return hashCode;
         }
      }

      public static bool operator ==(Slice left, Slice right) => left.Equals(right);

      public static bool operator !=(Slice left, Slice right) => !left.Equals(right);
   }
}