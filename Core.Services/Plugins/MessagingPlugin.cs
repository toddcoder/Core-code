using Core.Services.Plugins;
using Standard.Configurations;
using Standard.ObjectGraphs;
using Standard.Queuing;

namespace Standard.Services.Plugins
{
   public abstract class MessagingPlugin<TMessage> : Plugin, IDispatchSink<TMessage>
      where TMessage : class, IDisposition, new()
   {
      protected MessagePump<TMessage> pump;

      public MessagingPlugin(string name, Configuration configuration, ObjectGraph jobGroup)
         : base(name, configuration, jobGroup) => pump = new MessagePump<TMessage>(this);

      public override void SetUp()
      {
         base.SetUp();

         var messageGraph = jobGroup["messages"];
         pump.SetUp(messageGraph);
         pump.SetServiceMessage(serviceMessage);
      }

      public abstract void Request(TMessage message);

      public virtual void PerformDispatch() => Dispatch();

      public abstract void RequestCancel();

      public bool IsBusy { get; set; }

      public virtual void PreRequest(TMessage message) { }

      public virtual void PostRequest() { }

      public virtual void Busy() { }

      public virtual bool MessagesEnabled
      {
         get => pump.Enabled;
         set => pump.Enabled = value;
      }
   }
}