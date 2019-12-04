using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Enumerables;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Text
{
   public class DifferenceItem : EquatableBase
   {
      protected List<DifferenceItem> subItems;

      public DifferenceItem(string text, DifferenceType type, IMaybe<int> position)
      {
         Text = text;
         Type = type;
         Position = position;
         subItems = new List<DifferenceItem>();
      }

      public DifferenceItem(string text, DifferenceType type, int position) : this(text, type, position.Some()) { }

      public DifferenceItem(string text, DifferenceType type) : this(text, type, none<int>()) { }

      public DifferenceItem() : this("", DifferenceType.Imaginary) { }

      protected override bool equals(object other)
      {
         if (other is DifferenceItem otherDiffItem)
         {
            return Position.HasValue == otherDiffItem.Position.HasValue && subItemsEqual(otherDiffItem);
         }
         else
         {
            return false;
         }
      }

      [Equatable]
      public DifferenceType Type { get; set; }

      public IMaybe<int> Position { get; }

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
         var builder = new StringBuilder();
         builder.Append(Text.Elliptical(80, ' '));
         builder.Append('|');
         builder.Append(Type);

         if (Position.If(out var position))
         {
            builder.Append("@");
            builder.Append(position);
         }

         if (subItems.Count > 0)
         {
            builder.Append('(');
            builder.Append(subItems.Select(i => i.ToString()).Stringify());
            builder.Append(')');
         }

         return builder.ToString();
      }
   }
}