using System;
using Core.Assertions;
using Core.Exceptions;
using Core.Monads;
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
         activator.MustAs(nameof(activator)).Not.BeNull().Assert();

         if (!this.activator.HasValue)
         {
            this.activator = activator.Some();
         }
      }

      public void OverrideWith(Func<T> activator)
      {
         activator.MustAs(nameof(activator)).Not.BeNull().Assert();

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

               returnValue.MustAs(nameof(value)).Not.BeNull().Assert();

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