using System.Collections.Generic;
using System.IO;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Text
{
   public class DifferenceItem : EquatableBase
   {
      protected List<DifferenceItem> subItems;

      public DifferenceItem(string text, DifferenceType type, Maybe<int> position)
      {
         Text = text;
         Type = type;
         Position = position;
         subItems = new List<DifferenceItem>();
      }

      public DifferenceItem(string text, DifferenceType type, int position) : this(text, type, position.Some())
      {
      }

      public DifferenceItem(string text, DifferenceType type) : this(text, type, none<int>())
      {
      }

      public DifferenceItem() : this("", DifferenceType.Imaginary)
      {
      }

      protected override bool equals(object other)
      {
         return other is DifferenceItem otherDiffItem && Position.IsSome == otherDiffItem.Position.IsSome && subItemsEqual(otherDiffItem);
      }

      [Equatable]
      public DifferenceType Type { get; set; }

      public Maybe<int> Position { get; }

      [Equatable]
      public string Text { get; }

      public List<DifferenceItem> SubItems => subItems;

      protected bool subItemsEqual(DifferenceItem otherItem)
      {
         if (subItems.Count == 0)
         {
            return otherItem.SubItems.Count == 0;
         }
         else if (otherItem.SubItems.Count == 0)
         {
            return false;
         }

         if (subItems.Count != otherItem.SubItems.Count)
         {
            return false;
         }

         for (var i = 0; i < subItems.Count; i++)
         {
            if (!subItems[i].Equals(otherItem.SubItems[i]))
            {
               return false;
            }
         }

         return true;
      }

      public override string ToString()
      {
         using var writer = new StringWriter();

         if (Position.If(out var position))
         {
            writer.Write(position.RightJustify(10));
            writer.Write(" ");
         }
         else
         {
            writer.Write(" ".Repeat(11));
         }

         writer.Write(Type.LeftJustify(10));

         writer.Write(" | ");
         writer.Write(Text.Elliptical(60, ' '));

         if (subItems.Count > 0)
         {
            writer.WriteLine();
            writer.WriteLine("          {");
            foreach (var subItem in subItems)
            {
               writer.WriteLine($"  {subItem}");
            }

            writer.Write("          }");
         }

         return writer.ToString();
      }
   }
}