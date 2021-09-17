using System;
using System.Timers;
using Core.Services.Loggers;
using Standard.Computer;
using Standard.ObjectGraphs;
using Standard.Queuing;
using Standard.Types.Collections;
using Standard.Types.Dates;
using Standard.Types.Dates.DateIncrements;
using Standard.Types.Monads;
using static Standard.Types.Dates.Now.NowServer;
using static Standard.Types.Monads.MonadFunctions;

namespace Standard.Services.Plugins
{
   public class MessagePump<TMessage>
      where TMessage : class, IDisposition, new()
   {
      protected IDispatchSink<TMessage> sink;
      protected IMaybe<ServiceMessage> message;
      protected Timer timer;
      protected IMaybe<DateTime> stopTime;
      protected Queue<TMessage> queue;
      protected TimeSpan sleeping;

      public MessagePump(IDispatchSink<TMessage> sink)
      {
         this.sink = sink;
         message = none<ServiceMessage>();
         stopTime = none<DateTime>();
         sleeping = 500.Milliseconds();
         ExhaustQueue = false;
      }

      public string ApplicationName { get; set; }

      public FolderName RootFolder { get; set; }

      public TimeSpan TimerInterval { get; set; }

      protected void enableTimer(bool enabled)
      {
         stopTime = when(!enabled, () => Now);
         timer.Enabled = enabled;
      }

      public bool Enabled
      {
         get => timer.Enabled;
         set => enableTimer(value);
      }

      public bool ExhaustQueue { get; set; }

      public void SetServiceMessage(ServiceMessage message) => this.message = message.Some();

      void startDispatch(object sender, ElapsedEventArgs e)
      {
         enableTimer(false);

         if (stopTime.If(out var st) && e.SignalTime > st)
            return;

         try
         {
            Dispatch();
         }
         catch (Exception exception)
         {
            if (message.If(out var m))
               m.EmitException(exception);
         }
         finally
         {
            enableTimer(true);
         }
      }

      public virtual void Dispatch()
      {
         var timeout = new Timeout(5.Minutes());
         var count = 0;
         while (!timeout.Expired)
         {
            if (queue.Dequeue(sink.Name).If(out var item))
            {
               var obj = item.Object;
               if (obj is IDispatchType dispatchType)
               {
                  switch (dispatchType.DispatchType)
                  {
                     case DispatchType.Perform:
                        if (sink.IsBusy)
                        {
                           sink.Busy();
                           return;
                        }

                        sink.PreRequest(obj);
                        sink.PerformDispatch();
                        break;
                     case DispatchType.Cancel:
                        sink.PreRequest(obj);
                        sink.RequestCancel();
                        break;
                     case DispatchType.MessageBased:
                        if (sink.IsBusy)
                        {
                           sink.Busy();
                           return;
                        }

                        sink.PreRequest(obj);
                        sink.Request(obj);
                        break;
                  }
               }
               else
               {
                  sink.PreRequest(obj);
                  sink.Request(obj);
               }

               sink.PostRequest();
               if (++count % 20 == 0)
               {
                  sleeping.Sleep();
                  count = 0;
               }
            }

            if (!ExhaustQueue)
               break;
         }
      }

      public virtual void SetUp(ObjectGraph graph)
      {
         var settings = graph.QueueSettings();
         ApplicationName = settings.ApplicationName;
         RootFolder = settings.RootFolder;
         TimerInterval = graph.FlatMap("interval", g => g.Value.ToTimeSpan(), () => 30.Seconds());
         timer = new Timer(TimerInterval.TotalMilliseconds);
         timer.Elapsed += startDispatch;
         queue = new Queue<TMessage>(ApplicationName, RootFolder);
      }

      public Queue<TMessage> Queue => queue;

      public IDispatchSink<TMessage> Sink => sink;
   }
}