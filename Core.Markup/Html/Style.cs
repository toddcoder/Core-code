using System;
using Core.Matching;
using static Core.Monads.MonadFunctions;
using static Core.Objects.GetHashCodeGenerator;

namespace Core.Markup.Html;

public class Style : IEquatable<Style>
{
   public static implicit operator Style(string source)
   {
      var _result = source.Matches("^ /(-[':']+) /s* ':' /s* /(.+) $; f");
      if (_result)
      {
         var (key, value) = ~_result;
         return new Style(key, value);
      }
      else
      {
         throw fail($"Didn't understand style {source}");
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

   public override int GetHashCode() => hashCode() + Key + Value;

   public static bool operator ==(Style left, Style right) => Equals(left, right);

   public static bool operator !=(Style left, Style right) => !Equals(left, right);

   public override string ToString() => $"{Key}: {Value}";
}