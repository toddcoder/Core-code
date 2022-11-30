using System;
using static Core.Objects.GetHashCodeGenerator;

namespace Core.Strings.Text;

public class Difference : IEquatable<Difference>
{
   public static Difference FromDifferenceItem(DifferenceItem item)
   {
      var position = item.Position | -1;
      return new Difference(item.Text, item.Type, position);
   }

   public Difference(string text, DifferenceType type, int position)
   {
      Text = text;
      Type = type;
      Position = position;
   }

   public string Text { get; }

   public DifferenceType Type { get; }

   public int Position { get; }

   public override string ToString() => $"{Position.RightJustify(10)} | {Type.LeftJustify(9)} | {Text.Elliptical(60, ' ')}";

   public bool Equals(Difference other) => Text == other.Text && Type == other.Type && Position == other.Position;

   public override bool Equals(object obj) => obj is Difference other && Equals(other);

   public override int GetHashCode() => hashCode() + Text + Type + Position;

   public static bool operator ==(Difference left, Difference right) => Equals(left, right);

   public static bool operator !=(Difference left, Difference right) => !Equals(left, right);
}