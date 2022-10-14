using System;
using System.Text;
using Core.Exceptions;
using Core.Internet.Smtp;

namespace Core.Services.Loggers;

public class EmailMessage : IServiceMessage
{
   protected Address address;
   protected StringBuilder builder;

   public EmailMessage(Address address)
   {
      this.address = address;
      builder = new StringBuilder();
   }

   public void Begin() => builder.Clear();

   public void EmitException(Exception exception) => builder.Append($"Exception: {exception.DeepException()}");

   public void EmitExceptionAttempt(Exception exception, int retry)
   {
      var attempt = retry == 0 ? "Try" : $"Retry {retry}";
      builder.Append($"{attempt}: Exception: {exception.DeepException()}");
   }

   public void EmitMessage(object message) => builder.Append(message);

   public void EmitMessage(string message) => builder.Append(message);

   public void EmitExceptionMessage(object message) => builder.Append($"Exception: {message}");

   public void EmitExceptionMessage(string message) => builder.Append($"Exception: {message}");

   public void EmitWarning(Exception exception) { }

   public void EmitWarningMessage(object message) { }

   public void EmitWarningMessage(string message) { }

   public void Commit()
   {
      var emailer = new Emailer
      {
         Address = address,
         Body = builder.ToString(),
         Priority = PriorityType.High,
         UseCredentials = true
      };
      emailer.TryTo.SendText();
   }

   public bool DateEnabled { get; set; }
}