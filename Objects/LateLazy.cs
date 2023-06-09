using System;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

public class LateLazy<T>
{
   protected const string DEFAULT_ERROR_MESSAGE = "Activator has not been set";

   public static implicit operator T(LateLazy<T> lazy) => lazy.Value;

   protected bool overriding;
   protected string errorMessage;
   protected Maybe<T> _value;
   protected Maybe<Func<T>> _activator;

   public LateLazy(bool overriding = false, string errorMessage = DEFAULT_ERROR_MESSAGE)
   {
      this.overriding = overriding;
      this.errorMessage = errorMessage;

      _value = nil;
      _activator = nil;
   }

   public void ActivateWith(Func<T> activator)
   {
      activator.Must().Not.BeNull().OrThrow();

      if (!_activator || overriding)
      {
         _activator = activator;
         _value = nil;
         HasActivator = true;
      }
   }

   public T Value
   {
      get
      {
         if (_value is (true, var value))
         {
            return value;
         }
         else if (_activator is (true, var activator))
         {
            var returnValue = activator();
            returnValue.Must().Not.BeNull().OrThrow();
            _value = returnValue;

            return returnValue;
         }
         else
         {
            throw fail(errorMessage);
         }
      }
   }

   public bool IsActivated => _value;

   public Maybe<T> AnyValue => _value;

   public bool HasActivator { get; set; }

   public LateLazyTrying<T> TryTo => new(this);

   public string ErrorMessage
   {
      get => errorMessage;
      set => errorMessage = value;
   }

   public void Reset() => _value = nil;

   public void Activate()
   {
      _ = Value;
   }
}