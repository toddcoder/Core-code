using System;

namespace Core.Strings
{
   public struct Span : IEquatable<Span>
   {
      public string Text;
      public int Start;
      public int Stop;

      public Span(string text, int start, int stop) : this()
      {
         Text = text;
         Start = start;
         Stop = stop;
      }

      public void Deconstruct(out string text, out int start, out int stop)
      {
         text = Text;
         start = Start;
         stop = Stop;
      }

      public bool Equals(Span other) => Text == other.Text && Start == other.Start && Stop == other.Stop;

      public override bool Equals(object obj) => obj is Span other && Equals(other);

      public override int GetHashCode()
      {
         unchecked
         {
            var hashCode = Text != null ? Text.GetHashCode() : 0;
            hashCode = hashCode * 397 ^ Start;
            hashCode = hashCode * 397 ^ Stop;
            return hashCode;
         }
      }

      public static bool operator ==(Span left, Span right) => left.Equals(right);

      public static bool operator !=(Span left, Span right) => !left.Equals(right);
   }
}