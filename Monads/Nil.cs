using System;

namespace Core.Monads
{
   public class Nil : IEquatable<Nil>
   {
      [Obsolete("Use appropriate nil variable")]
      public static Maybe<T> Of<T>() => new None<T>();

      protected int hashCode;

      public Nil()
      {
         hashCode = typeof(Nil).GetHashCode();
      }

      public bool Equals(Nil other) => true;

      public override bool Equals(object obj) => obj is Nil;

      public override int GetHashCode() => hashCode;

      public static bool operator ==(Nil left, Nil right) => Equals(left, right);

      public static bool operator !=(Nil left, Nil right) => !Equals(left, right);
   }
}