using System;
using Core.Assertions;
using Core.Exceptions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Objects
{
   public class LateLazy<T>
   {
      protected const string DEFAULT_ERROR_MESSAGE = "Activator has not been set";

      protected bool overriding;
      protected string errorMessage;
      protected Maybe<T> _value;
      protected Maybe<Func<T>> _activator;

      public LateLazy(bool overriding = false, string errorMessage = DEFAULT_ERROR_MESSAGE)
      {
         this.overriding = overriding;
         this.errorMessage = errorMessage;

         _value = none<T>();
         _activator = none<Func<T>>();
      }

      public void ActivateWith(Func<T> activator)
      {
         activator.Must().Not.BeNull().OrThrow();

         if (_activator.IsNone || overriding)
         {
            _activator = activator.Some();
            _value = none<T>();
         }
      }

      public T Value
      {
         get
         {
            if (_value.If(out var value))
            {
               return value;
            }
            else if (_activator.If(out var activator))
            {
               var returnValue = activator();
               returnValue.Must().Not.BeNull().OrThrow();
               _value = returnValue.Some();

               return returnValue;
            }
            else
            {
               throw errorMessage.Throws();
            }
         }
      }

      public bool IsActivated => _value.IsSome;

      public Maybe<T> AnyValue => _value;

      public LateLazyTrying<T> TryTo => new(this);

      public string ErrorMessage
      {
         get => errorMessage;
         set => errorMessage = value;
      }
   }
}