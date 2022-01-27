using System;
using Core.Exceptions;
using Core.Matching;

namespace Core.Markup.Html
{
   public class Style : IEquatable<Style>
   {
      public static implicit operator Style(string source)
      {
         if (source.Matches("^ /(-[':']+) /s* ':' /s* /(.+) $; f").If(out var result))
         {
            return new Style(result.FirstGroup, result.SecondGroup);
         }
         else
         {
            throw $"Didn't understand style {source}".Throws();
         }
      }

      public Style(string key, string value)
      {
         Key = key;
         Value = value;
      }

      public string Key { get; }

      public string Value { get; }

      public bool Equals(Style other)
      {
         if (other is null)
         {
            return false;
         }
         else
         {
            return Key == other.Key && Value == other.Value;
         }
      }

      public override bool Equals(object obj)
      {
         return obj is Style other && Equals(other);
      }

      public override int GetHashCode()
      {
         unchecked
         {
            return (Key != null ? Key.GetHashCode() : 0) * 397 ^ (Value != null ? Value.GetHashCode() : 0);
         }
      }

      public static bool operator ==(Style left, Style right) => Equals(left, right);

      public static bool operator !=(Style left, Style right) => !Equals(left, right);

      public override string ToString() => $"{Key}: {Value}";
   }
}