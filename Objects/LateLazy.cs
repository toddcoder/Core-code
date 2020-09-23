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
      const string DEFAULT_ERROR_MESSAGE = "Activator has not been set";

      protected bool overriding;
      protected string errorMessage;
      protected IMaybe<T> value;
      protected IMaybe<Func<T>> activator;

      public LateLazy(bool overriding = false, string errorMessage = DEFAULT_ERROR_MESSAGE)
      {
         this.overriding = overriding;
         this.errorMessage = errorMessage;

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
               throw errorMessage.Throws();
            }
         }
      }

      public bool IsActivated => value.HasValue;

      public IMaybe<T> AnyValue => value;

      public LateLazyTrying<T> TryTo => new LateLazyTrying<T>(this);

      public string ErrorMessage
      {
         get => errorMessage;
         set => errorMessage = value;
      }
   }
}