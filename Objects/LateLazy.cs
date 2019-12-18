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

      public void ActivateWith(Func<T> localActivator)
      {
         localActivator.Must().Not.BeNull().Assert("Activator can't be null");

         if (!activator.HasValue)
         {
            activator = localActivator.Some();
         }
      }

      public void OverrideWith(Func<T> localActivator)
      {
         localActivator.Must().Not.BeNull().Assert("Activator can't be null");

         activator = localActivator.Some();
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

               returnValue.Must().Not.BeNull().Assert("Activator can't return a null value");

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