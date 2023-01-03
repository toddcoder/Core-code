using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

public abstract class EventHandlerStore<TEventArgs> where TEventArgs : EventArgs
{
   protected Control control;
   protected EventInfo eventInfo;
   protected MethodInfo methodInfo;
   protected List<Delegate> handlers;

   public EventHandlerStore(Control control, string eventName)
   {
      this.control = control;
      eventInfo = control.GetType().GetEvent(eventName);

      methodInfo = typeof(EventHandlerStore<TEventArgs>).GetMethod("Handler");
      handlers = new List<Delegate>();
   }

   public abstract void Handler(object sender, TEventArgs e);

   protected Delegate getDelegate() => Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);

   public void Subscribe()
   {
      var @delegate = getDelegate();
      eventInfo.AddEventHandler(control, @delegate);
      handlers.Add(@delegate);
   }

   public void Unsubscribe()
   {
      foreach (var handler in handlers)
      {
         eventInfo.RemoveEventHandler(control, handler);
      }
   }
}

public abstract class EventHandlerStore : EventHandlerStore<EventArgs>
{
   public EventHandlerStore(Control control, string eventName) : base(control, eventName)
   {
   }
}