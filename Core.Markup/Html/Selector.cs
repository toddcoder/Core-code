using System;
using System.Collections.Generic;
using Core.Enumerables;
using Core.Exceptions;
using Core.Matching;
using Core.Strings;

namespace Core.Markup.Html
{
   public class Selector : IEquatable<Selector>
   {
      public static implicit operator Selector(string source)
      {
         if (source.Matches("^ /(-['{']+) /s* '{' (/s* /(.+))? $; f").Map(out var result))
         {
            var name = result.FirstGroup;
            var selector = new Selector(name);

            var styleSource = result.SecondGroup;
            if (styleSource.IsNotEmpty())
            {
               Style style = styleSource;
               selector.styles.Add(style);
            }

            return selector;
         }
         else
         {
            throw $"Didn't understand selector {source}".Throws();
         }
      }

      public static Selector operator +(Selector selector, Style style)
      {
         selector.Add(style);
         return selector;
      }

      protected List<Style> styles;

      public Selector(string name)
      {
         Name = name;

         styles = new List<Style>();
      }

      public string Name { get; }

      public void Add(Style style) => styles.Add(style);

      public bool Equals(Selector other)
      {
         if (other is null)
         {
            return false;
         }
         else
         {
            return Equals(styles, other.styles) && Name == other.Name;
         }
      }

      public override bool Equals(object obj)
      {
         return obj is Selector other && Equals(other);
      }

      public override int GetHashCode()
      {
         unchecked
         {
            return (styles != null ? styles.GetHashCode() : 0) * 397 ^ (Name != null ? Name.GetHashCode() : 0);
         }
      }

      public static bool operator ==(Selector left, Selector right) => Equals(left, right);

      public static bool operator !=(Selector left, Selector right) => !Equals(left, right);

      public override string ToString() => $"{Name} {{ {styles.ToString("; ")} }}";
   }
}