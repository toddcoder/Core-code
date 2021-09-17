using System;
using Core.Internet.Smtp;

namespace Core.Services.Loggers
{
   public class ServiceWriter : IServiceWriter
   {
      public static void SendAsEmail(Address address, string message) => new Emailer
      {
         Address = address,
         Body = message,
         Priority = PriorityType.High
      }.TryTo.SendHtml();

      protected ServiceMessage serviceMessage;
      protected Address address;

      public ServiceWriter(ServiceMessage serviceMessage, Address address)
      {
         this.serviceMessage = serviceMessage;
         this.address = address;
      }

      public void WriteRaw(string text) => serviceMessage.EmitMessage(text);

      public void Write(string message) => serviceMessage.EmitMessage(message);

      public void Write(object message) => serviceMessage.EmitMessage(message);

      public void WriteLine(string message) => serviceMessage.EmitMessage(message);

      public void WriteLine(object message) => serviceMessage.EmitMessage(message);

      public void WriteException(Exception exception) => serviceMessage.EmitException(exception);

      public void WriteException(string message) => serviceMessage.EmitExceptionMessage(message);

      public void WriteException(object message) => serviceMessage.EmitExceptionMessage(message);

      public void WriteExceptionLine(Exception exception) => serviceMessage.EmitException(exception);

      public void WriteExceptionLine(string message) => serviceMessage.EmitExceptionMessage(message);

      public void WriteExceptionLine(object message) => serviceMessage.EmitExceptionMessage(message);

      public void SendEmail(Address address, string message) => SendAsEmail(address, message);

      public void SendEmail(string message) => SendEmail(address, message);
   }
}