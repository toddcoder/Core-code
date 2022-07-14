using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Services.Loggers
{
   public class ServiceMessage : IServiceMessage
   {
      protected List<IServiceMessage> messages;
      protected Maybe<ServiceLogger> _logger;
      protected Maybe<NamedExceptions> _namedExceptions;
      protected Maybe<IServiceWriter> _serviceWriter;
      protected Maybe<EventWriter> _eventWriter;
      protected bool autoBegin;
      protected Maybe<StringBuilder> _errorMessage;

      public ServiceMessage(string applicationName)
      {
         try
         {
            _eventWriter = new EventWriter(applicationName);
         }
         catch
         {
            _eventWriter = nil;
         }

         messages = new List<IServiceMessage>();
         _logger = nil;
         _namedExceptions = nil;
         _serviceWriter = nil;
         autoBegin = false;
         _errorMessage = nil;
      }

      public Maybe<ServiceLogger> Logger => _logger;

      public Maybe<NamedExceptions> NamedExceptions => _namedExceptions;

      public Maybe<IServiceWriter> ServiceWriter => _serviceWriter;

      public bool AutoBegin
      {
         set => autoBegin = value;
      }

      public void Add(IServiceMessage serviceMessage)
      {
         messages.Add(serviceMessage);

         if (!_logger && serviceMessage is ServiceLogger serviceLogger)
         {
            _logger = serviceLogger;
         }

         if (!_namedExceptions && serviceMessage is NamedExceptions namedExceptions)
         {
            _namedExceptions = namedExceptions;
         }
      }

      public void AddPossible(IServiceWriter serviceWriter)
      {
         if (!_serviceWriter && serviceWriter is IServiceMessage serviceMessage)
         {
            Add(serviceMessage);
            _serviceWriter = serviceWriter.Some();
         }
      }

      public IEnumerable<IServiceMessage> ServiceMessages => messages;

      protected void beginIfAutoBegin()
      {
         if (autoBegin)
         {
            Begin();
            autoBegin = false;
         }
      }

      public void Begin()
      {
         try
         {
            foreach (var serviceMessage in messages)
            {
               serviceMessage.Begin();
            }
         }
         catch (Exception exception)
         {
            if (_eventWriter.Map(out var eventWriter))
            {
               eventWriter.EmitException(exception);
            }
         }
      }

      public void EmitException(Exception exception)
      {
         beginIfAutoBegin();

         foreach (var serviceMessage in messages)
         {
            try
            {
               serviceMessage.EmitException(exception);
            }
            catch (Exception innerException)
            {
               if (_eventWriter.Map(out var eventWriter))
               {
                  eventWriter.EmitException(exception);
                  eventWriter.EmitException(innerException);
               }
            }
         }
      }

      public void EmitExceptionAttempt(Exception exception, int retry)
      {
         beginIfAutoBegin();

         foreach (var serviceMessage in messages)
         {
            try
            {
               serviceMessage.EmitExceptionAttempt(exception, retry);
            }
            catch (Exception innerException)
            {
               if (_eventWriter.Map(out var eventWriter))
               {
                  eventWriter.EmitExceptionAttempt(exception, retry);
                  eventWriter.EmitException(innerException);
               }
            }
         }
      }

      protected bool emitMessage(object message)
      {
         var strMessage = message?.ToString() ?? "";
         if (strMessage.StartsWith("!"))
         {
            if (!_errorMessage)
            {
               _errorMessage = new StringBuilder();
            }

            if (_errorMessage.Map(out var errorMessage))
            {
               errorMessage.AppendLine(strMessage.Drop(1));
            }

            return false;
         }
         else if (_errorMessage.Map(out var errorMessage))
         {
            EmitExceptionMessage(errorMessage.ToString());
            return true;
         }
         else
         {
            return true;
         }
      }

      public void EmitMessage(object message)
      {
         beginIfAutoBegin();

         if (emitMessage(message))
         {
            foreach (var serviceMessage in messages)
            {
               try
               {
                  serviceMessage.EmitMessage(message);
               }
               catch (Exception exception)
               {
                  if (_eventWriter.Map(out var eventWriter))
                  {
                     eventWriter.EmitMessage(message);
                     eventWriter.EmitException(exception);
                  }
               }
            }
         }
      }

      public void EmitMessage(string message)
      {
         beginIfAutoBegin();

         if (emitMessage(message))
         {
            foreach (var serviceMessage in messages)
            {
               try
               {
                  serviceMessage.EmitMessage(message);
               }
               catch (Exception exception)
               {
                  if (_eventWriter.Map(out var eventWriter))
                  {
                     eventWriter.EmitMessage(message);
                     eventWriter.EmitException(exception);
                  }
               }
            }
         }
      }

      public void EmitExceptionMessage(object message)
      {
         beginIfAutoBegin();

         foreach (var serviceMessage in messages)
         {
            try
            {
               serviceMessage.EmitExceptionMessage(message);
            }
            catch (Exception exception)
            {
               if (_eventWriter.Map(out var eventWriter))
               {
                  eventWriter.EmitExceptionMessage(message);
                  eventWriter.EmitException(exception);
               }
            }
         }
      }

      public void EmitExceptionMessage(string message)
      {
         beginIfAutoBegin();

         foreach (var serviceMessage in messages)
         {
            try
            {
               serviceMessage.EmitExceptionMessage(message);
            }
            catch (Exception exception)
            {
               if (_eventWriter.Map(out var eventWriter))
               {
                  eventWriter.EmitExceptionMessage(message);
                  eventWriter.EmitException(exception);
               }
            }
         }
      }

      public void EmitWarning(Exception exception)
      {
         beginIfAutoBegin();

         foreach (var serviceMessage in messages)
         {
            try
            {
               serviceMessage.EmitWarning(exception);
            }
            catch (Exception e)
            {
               if (_eventWriter.Map(out var eventWriter))
               {
                  eventWriter.EmitWarning(exception);
                  eventWriter.EmitException(e);
               }
            }
         }
      }

      public void EmitWarningMessage(object message)
      {
         beginIfAutoBegin();

         foreach (var serviceMessage in messages)
         {
            try
            {
               serviceMessage.EmitWarningMessage(message);
            }
            catch (Exception e)
            {
               if (_eventWriter.Map(out var eventWriter))
               {
                  eventWriter.EmitWarningMessage(message);
                  eventWriter.EmitException(e);
               }
            }
         }
      }

      public void EmitWarningMessage(string message)
      {
         beginIfAutoBegin();
         foreach (var serviceMessage in messages)
         {
            try
            {
               serviceMessage.EmitWarningMessage(message);
            }
            catch (Exception e)
            {
               if (_eventWriter.Map(out var eventWriter))
               {
                  eventWriter.EmitWarningMessage(message);
                  eventWriter.EmitException(e);
               }
            }
         }
      }

      public void EmitResult<T>(Result<T> _result, Func<T, string> ifSuccessful)
      {
         beginIfAutoBegin();

         switch (_result)
         {
            case Success<T> success:
               EmitMessage(ifSuccessful(success.Value));
               break;
            case Failure<T> failure:
               EmitException(failure.Exception);
               break;
         }
      }

      public void EmitResult<T>(Result<T> _result, Func<T, string> ifSuccessful, Func<Exception, string> ifFailure)
      {
         beginIfAutoBegin();

         switch (_result)
         {
            case Success<T> success:
               EmitMessage(ifSuccessful(success.Value));
               break;
            case Failure<T> failure:
               EmitException(failure.Exception);
               break;
         }
      }

      public void EmitSuccess<T>(Result<T> _result, Func<T, string> ifSuccessful)
      {
         beginIfAutoBegin();

         if (_result.Map(out var result))
         {
            EmitMessage(ifSuccessful(result));
         }
      }

      public void EmitFailure<T>(Result<T> _result)
      {
         beginIfAutoBegin();

         if (_result.UnMap(out var exception))
         {
            EmitException(exception);
         }
      }

      public void EmitFailure<T>(Result<T> _result, Func<Exception, string> ifFailure)
      {
         beginIfAutoBegin();

         if (_result.UnMap(out var exception))
         {
            EmitException(exception);
            EmitExceptionMessage(ifFailure(exception));
         }
      }

      public void Commit()
      {
         if (autoBegin)
         {
            autoBegin = false;
            return;
         }

         foreach (var serviceMessage in messages)
         {
            try
            {
               serviceMessage.Commit();
            }
            catch (Exception exception)
            {
               if (_eventWriter.Map(out var eventWriter))
               {
                  eventWriter.EmitException(exception);
               }
            }
         }

         autoBegin = false;
      }

      public bool DateEnabled
      {
         get => messages.Any(m => m.DateEnabled);
         set
         {
            foreach (var serviceMessage in messages)
            {
               serviceMessage.DateEnabled = value;
            }
         }
      }
   }
}