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
      IMaybe<T> value;
      IMaybe<Func<T>> activator;

      public LateLazy()
      {
         value = none<T>();
         activator = none<Func<T>>();
      }

      public void ActivateWith(Func<T> activator)
      {
         assert(() => activator).Must().Not.BeNull().OrThrow();

         if (!this.activator.HasValue)
         {
            this.activator = activator.Some();
         }
      }

      public void OverrideWith(Func<T> activator)
      {
         assert(() => activator).Must().Not.BeNull().OrThrow();

         this.activator = activator.Some();
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
   }
}