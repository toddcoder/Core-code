using Core.Objects;

namespace Core.Strings.Text
{
   public class Difference : EquatableBase
   {
      public static Difference FromDifferenceItem(DifferenceItem item)
      {
         var position = item.Position.DefaultTo(() => -1);
         return new Difference(item.Text, item.Type, position);
      }
      public Difference(string text, DifferenceType type, int position)
      {
         Text = text;
         Type = type;
         Position = position;
      }

      [Equatable]
      public string Text { get; }

      [Equatable]
      public DifferenceType Type { get; }

      [Equatable]
      public int Position { get; }

      public override string ToString() => $"{Position.RightJustify(10)} | {Type.LeftJustify(9)} | {Text.Elliptical(60, ' ')}";
   }
}