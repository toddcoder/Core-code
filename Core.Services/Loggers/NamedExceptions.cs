using System;
using System.Collections;
using System.Collections.Generic;
using Core.Exceptions;
using Core.Internet.Smtp;

namespace Core.Services.Loggers;

public class NamedExceptions : IEnumerable<NamedException>, IServiceMessage
{
   protected Address address;
   protected string name;
   protected string title;
   protected int retryLimit;
   protected List<NamedException> namedExceptions;

   public NamedExceptions(Address address, string name, string title, int retryLimit)
   {
      this.address = address;
      this.name = name;
      this.title = title;
      this.retryLimit = retryLimit;

      namedExceptions = new List<NamedException>();
   }

   public void Add(Exception exception)
   {
      namedExceptions.Add(new NamedException
      {
         Name = name,
         Exception = exception
      });
   }

   public void AddTry(Exception exception, int retry)
   {
      namedExceptions.Add(new NamedException
      {
         Name = retry == 0 ? name : $"{name} - Retry {retry}",
         Exception = exception
      });
   }

   public void SendMessage()
   {
      try
      {
         if (namedExceptions.Count > 0)
         {
            var message = new ErrorEmailMessage(title);
            foreach (var namedException in namedExceptions)
            {
               message.Add(namedException.Name, namedException.Exception.DeepException());
            }

            ServiceWriter.SendAsEmail(address, message.ToString());
         }
      }
      finally
      {
         Clear();
      }
   }

   public void Log(ServiceLogger logger)
   {
      foreach (var namedException in namedExceptions)
      {
         logger.WriteExceptionLine(namedException.Exception);
      }
   }

   public void AddTo(NamedExceptions other)
   {
      foreach (var exception in namedExceptions)
      {
         other.Add(exception.Exception);
      }

      Clear();
   }

   public void Clear() => namedExceptions.Clear();

   public IEnumerator<NamedException> GetEnumerator() => namedExceptions.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Begin() => Clear();

   public void EmitException(Exception exception) => Add(exception);

   public void EmitExceptionAttempt(Exception exception, int retry) => AddTry(exception, retry);

   public void EmitMessage(object message) { }

   public void EmitMessage(string message) { }

   public void EmitExceptionMessage(object message) => EmitExceptionMessage(message?.ToString() ?? "");

   public void EmitExceptionMessage(string message) => Add(new Exception(message));

   public void EmitWarning(Exception exception) { }

   public void EmitWarningMessage(object message) { }

   public void EmitWarningMessage(string message) { }

   public void Commit()
   {
      var count = namedExceptions.Count;
      if (count >= retryLimit || count == 0)
      {
         SendMessage();
      }
   }

   public bool DateEnabled { get; set; }

   public int Count => namedExceptions.Count;
}