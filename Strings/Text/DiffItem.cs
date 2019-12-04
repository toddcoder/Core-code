using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Enumerables;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Text
{
   public class DiffItem : EquatableBase
   {
      List<DiffItem> subItems;

      public DiffItem(string text, DiffType type, IMaybe<int> position)
      {
         Text = text;
         Type = type;
         Position = position;
         subItems = new List<DiffItem>();
      }

      public DiffItem(string text, DiffType type, int position) : this(text, type, position.Some()) { }

      public DiffItem(string text, DiffType type) : this(text, type, none<int>()) { }

      public DiffItem() : this("", DiffType.Imaginary) { }

      protected override bool equals(object other)
      {
         if (other is DiffItem otherDiffItem)
         {
            return Position.HasValue == otherDiffItem.Position.HasValue && subItemsEqual(otherDiffItem);
         }
         else
         {
            return false;
         }
      }

      [Equatable]
      public DiffType Type { get; set; }

      public IMaybe<int> Position { get; }

      [Equatable]
      public string Text { get; }

      public List<DiffItem> SubItems => subItems;

      protected bool subItemsEqual(DiffItem otherItem)
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