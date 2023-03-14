using System;

namespace Core.Monads;

public class Nil : IEquatable<Nil>
{
   public static explicit operator Nil(string message) => new NilWithMessage(message);

   protected Lazy<int> hashCode;

   public Nil()
   {
      hashCode = new Lazy<int>(() => typeof(Nil).GetHashCode());
   }

   public bool Equals(Nil other) => true;

   public override bool Equals(object obj) => obj is Nil;

   public override int GetHashCode() => hashCode.Value;

   public static bool operator ==(Nil left, Nil right) => Equals(left, right);

   public static bool operator !=(Nil left, Nil right) => !Equals(left, right);
}