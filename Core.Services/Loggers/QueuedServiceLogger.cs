using System;
using Core.Computers;
using Core.Configurations;
using Core.DataStructures;
using Core.Exceptions;
using Core.Monads;

namespace Core.Services.Loggers
{
   public class QueuedServiceLogger : ServiceLogger
   {
      protected class QueueItem
      {
         protected string text;
         protected bool newLine;
         protected bool exception;

         public QueueItem(string text, bool newLine, bool exception)
         {
            this.text = text;
            this.newLine = newLine;
            this.exception = exception;
         }

         public string Text => text;

         public bool NewLine => newLine;

         public bool Exception => exception;
      }

      protected MaybeQueue<QueueItem> queue;

      public new static Result<QueuedServiceLogger> FromConfiguration(Configuration configuration)
      {
         Result<ServiceLogger> creator(FolderName baseFolder, string jobName, int sizeLimit, TimeSpan expiry, Maybe<EventLogger> _eventLogger)
         {
            return new QueuedServiceLogger(baseFolder, jobName, sizeLimit, expiry, _eventLogger);
         }

         return fromConfiguration(configuration, creator).Map(sl => (QueuedServiceLogger)sl);
      }

      public QueuedServiceLogger(FolderName baseFolder, string jobName, int sizeLimit, TimeSpan expiry, Maybe<EventLogger> _eventLogger) :
         base(baseFolder, jobName, sizeLimit, expiry, _eventLogger)
      {
         queue = new MaybeQueue<QueueItem>();
      }

      protected void dequeue()
      {
         lock (queue)
         {
            while (queue.Dequeue().If(out var item))
            {
               baseWrite(item);
            }
         }
      }

      protected virtual void baseWrite(QueueItem item)
      {
         if (item.NewLine)
         {
            base.WriteLine(item.Exception ? $"< {item.Text} >" : item.Text);
         }
         else if (item.Exception)
         {
            base.Write($"< {item.Text} >");
         }
         else
         {
            base.Write(item.Text);
         }
      }

      protected void enqueue(string text, bool newLine, bool exception)
      {
         var item = new QueueItem(text, newLine, exception);
         lock (queue)
         {
            if (queue.Count == 0)
            {
               baseWrite(item);
            }
            else
            {
               queue.Enqueue(item);
            }
         }

         dequeue();
      }

      public override void WriteLine(string message) => enqueue(message, true, false);

      public override void Write(string message) => enqueue(message, false, false);

      public override void WriteExceptionLine(Exception exception) => enqueue(exception.DeepMessage(), true, true);

      public override void WriteException(Exception exception) => enqueue(exception.DeepMessage(), false, true);

      public override void Commit() => dequeue();
   }
}