﻿using System;
using System.Collections.Generic;

namespace Standard.Types.Monads
{
   internal class ResultIterator<T>
   {
      IEnumerable<IResult<T>> enumerable;
      IMaybe<Action<T>> success;
      IMaybe<Action<Exception>> failure;

      public ResultIterator(IEnumerable<IResult<T>> enumerable, Action<T> ifSuccess = null,
         Action<Exception> ifFailure = null)
      {
         this.enumerable = enumerable;
         success = ifSuccess.SomeIfNotNull();
         failure = ifFailure.SomeIfNotNull();
      }

      void handle(IResult<T> result)
      {
         if (result.If(out var value, out var exception))
         {
            if (success.If(out var action))
               action(value);
         }
         else if (failure.If(out var faction))
            faction(exception);
      }

      public IEnumerable<IResult<T>> All()
      {
         foreach (var result in enumerable)
            handle(result);

         return enumerable;
      }

      public IEnumerable<T> SuccessesOnly()
      {
         var list = new List<T>();

         foreach (var result in enumerable)
         {
            handle(result);
            if (result.If(out var t))
               list.Add(t);
         }

         return list;
      }

      public IEnumerable<Exception> FailuresOnly()
      {
         var list = new List<Exception>();

         foreach (var result in enumerable)
         {
            handle(result);
            if (result.IfNot(out var e))
               list.Add(e);
         }

         return list;
      }

      public (IEnumerable<T> enumerable, IMaybe<Exception> exception) SuccessesThenFailure()
      {
         var list = new List<T>();

         foreach (var result in enumerable)
         {
            handle(result);
            if (result.If(out var value, out var exception))
               list.Add(value);
            else
               return (list, exception.Some());
         }

         return (list, MonadFunctions.none<Exception>());
      }

      public IResult<IEnumerable<T>> IfAllSuccesses()
      {
         var list = new List<T>();

         foreach (var result in enumerable)
         {
            handle(result);
            if (result.Out(out var value, out var original))
               list.Add(value);
            else
               return original.ExceptionAs<IEnumerable<T>>();
         }

         return MonadFunctions.success<IEnumerable<T>>(list);
      }
   }
}