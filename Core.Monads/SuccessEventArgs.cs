using System;

namespace Standard.Types.Monads
{
   public class SuccessEventArgs<T> : EventArgs
   {
      public SuccessEventArgs(T value) => Value = value;

      public T Value { get; }
   }
}