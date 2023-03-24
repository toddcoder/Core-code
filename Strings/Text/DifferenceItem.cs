using System;
using System.Collections.Generic;
using System.IO;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;
using static Core.Objects.GetHashCodeGenerator;

namespace Core.Strings.Text;

public class DifferenceItem : IEquatable<DifferenceItem>
{
   protected List<DifferenceItem> subItems;

   public DifferenceItem(string text, DifferenceType type, Optional<int> position)
   {
      Text = text;
      Type = type;
      Position = position;
      subItems = new List<DifferenceItem>();
   }

   public DifferenceItem(string text, DifferenceType type, int position) : this(text, type, position.Some())
   {
   }

   public DifferenceItem(string text, DifferenceType type) : this(text, type, nil)
   {
   }

   public DifferenceItem() : this("", DifferenceType.Imaginary)
   {
   }

   /*protected override bool equals(object other)
   {
      return other is DifferenceItem otherDiffItem && (bool)Position == (bool)otherDiffItem.Position && subItemsEqual(otherDiffItem);
   }*/

   [Equatable]
   public DifferenceType Type { get; set; }

   public Optional<int> Position { get; }

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

      if (Position is (true, var position))
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

   public bool Equals(DifferenceItem other) => (bool)Position == (bool)other.Position && subItemsEqual(other);

   public override bool Equals(object obj) => obj is DifferenceItem other && Equals(other);

   public override int GetHashCode() => hashCode() + subItems + Type + Position + Text;

   public static bool operator ==(DifferenceItem left, DifferenceItem right) => Equals(left, right);

   public static bool operator !=(DifferenceItem left, DifferenceItem right) => !Equals(left, right);
}