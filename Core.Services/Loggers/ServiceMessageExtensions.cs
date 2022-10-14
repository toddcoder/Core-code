using System;
using Core.Monads;

namespace Core.Services.Loggers;

public static class ServiceMessageExtensions
{
   public static Result<T> EmitResult<T>(this Result<T> _result, ServiceMessage message, Func<T, string> ifSuccessful)
   {
      message.EmitResult(_result, ifSuccessful);
      return _result;
   }

   public static Result<T> EmitResult<T>(this Result<T> _result, ServiceMessage message, Func<T, string> ifSuccessful,
      Func<Exception, string> ifFailure)
   {
      message.EmitResult(_result, ifSuccessful, ifFailure);
      return _result;
   }

   public static Result<T> EmitSuccess<T>(this Result<T> _result, ServiceMessage message, Func<T, string> ifSuccessful)
   {
      message.EmitSuccess(_result, ifSuccessful);
      return _result;
   }

   public static Result<T> EmitFailure<T>(this Result<T> _result, ServiceMessage message)
   {
      message.EmitFailure(_result);
      return _result;
   }

   public static Result<T> EmitFailure<T>(this Result<T> _result, ServiceMessage message, Func<Exception, string> ifFailure)
   {
      message.EmitFailure(_result, ifFailure);
      return _result;
   }
}