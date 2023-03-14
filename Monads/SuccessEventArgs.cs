using System;

namespace Core.Monads;

public class SuccessEventArgs<T> : EventArgs
{
   public SuccessEventArgs(T value) => Value = value;

   public T Value { get; }
}