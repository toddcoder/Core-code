using System;

namespace Core.Services.Loggers;

public interface IServiceMessage
{
   void Begin();

   void EmitException(Exception exception);

   void EmitExceptionAttempt(Exception exception, int retry);

   void EmitMessage(object message);

   void EmitMessage(string message);

   void EmitExceptionMessage(object message);

   void EmitExceptionMessage(string message);

   void EmitWarning(Exception exception);

   void EmitWarningMessage(object message);

   void EmitWarningMessage(string message);

   void Commit();

   bool DateEnabled { get; set; }
}