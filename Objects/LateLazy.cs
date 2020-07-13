using System;
using Core.Assertions;
using Core.Exceptions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Objects
{
   public class LateLazy<T>
   {
      protected bool overriding;
      protected IMaybe<T> value;
      protected IMaybe<Func<T>> activator;

      public LateLazy(bool overriding = false)
      {
         this.overriding = overriding;
         value = none<T>();
         activator = none<Func<T>>();
      }

      public void ActivateWith(Func<T> activator)
      {
         assert(() => activator).Must().Not.BeNull().OrThrow();

         if (!this.activator.HasValue || overriding)
         {
            this.activator = activator.Some();
            value = none<T>();
         }
      }

      public T Value
      {
         get
         {
            if (value.If(out var v))
            {
               return v;
            }
            else if (activator.If(out var f))
            {
               var returnValue = f();

               assert(() => returnValue).Must().Not.BeNull().OrThrow();

               value = returnValue.Some();

               return returnValue;
            }
            else
            {
               throw "Activator has not been set".Throws();
            }
         }
      }

      public LateLazyTrying<T> TryTo => new LateLazyTrying<T>(this);
   }
}