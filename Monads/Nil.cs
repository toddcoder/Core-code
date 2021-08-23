using System;

namespace Core.Monads
{
   public class Nil : IEquatable<Nil>
   {
      public bool Equals(Nil other) => true;

      public override bool Equals(object obj) => obj is Nil;

      public override int GetHashCode() => "nil".GetHashCode();

      public static bool operator ==(Nil left, Nil right) => Equals(left, right);

      public static bool operator !=(Nil left, Nil right) => !Equals(left, right);
   }
}