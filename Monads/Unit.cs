using System;

namespace Core.Monads
{
   public sealed class Unit : IEquatable<Unit>
   {
      private readonly int hashCode;

      public Unit()
      {
         hashCode = typeof(Unit).GetHashCode();
      }

      public bool Equals(Unit other) => true;

      public override bool Equals(object obj) => obj is Unit;

      public override int GetHashCode() => hashCode;

      public static bool operator ==(Unit left, Unit right) => Equals(left, right);

      public static bool operator !=(Unit left, Unit right) => !Equals(left, right);
   }
}