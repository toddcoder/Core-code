using System;
using System.Collections.Generic;
using static Core.Objects.GetHashCodeGenerator;

namespace Core.Objects;

public class TaggedValue<T> : IEquatable<TaggedValue<T>>
{
   public TaggedValue(string tag, T value)
   {
      Tag = tag;
      Value = value;
   }

   public string Tag { get; }

   public T Value { get; }

   public void Deconstruct(out string tag, out T value)
   {
      tag = Tag;
      value = Value;
   }

   public bool Equals(TaggedValue<T> other) => Tag == other.Tag && EqualityComparer<T>.Default.Equals(Value, other.Value);

   public override bool Equals(object obj) => obj is TaggedValue<T> other && Equals(other);

   public override int GetHashCode() => hashCode() + Tag + Value;

   public static bool operator ==(TaggedValue<T> left, TaggedValue<T> right) => Equals(left, right);

   public static bool operator !=(TaggedValue<T> left, TaggedValue<T> right) => !Equals(left, right);
}