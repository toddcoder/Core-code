using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Collections;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Messaging;

public static class MessageQueue
{
   private static AutoStringHash<List<IMessageQueueListener>> listeners;
   private static AutoStringHash<List<IMessageQueueSyncListener>> syncListeners;

   static MessageQueue()
   {
      listeners = new AutoStringHash<List<IMessageQueueListener>>(true, _ => new List<IMessageQueueListener>(), true);
      syncListeners = new AutoStringHash<List<IMessageQueueSyncListener>>(true, _ => new List<IMessageQueueSyncListener>(), true);
   }

   public static void Send(string sender, Message message)
   {
      var (subject, cargo) = message;
      foreach (var messageListener in listeners[sender])
      {
         Task.Run(() => messageListener.MessageFrom(sender, subject, cargo));
      }
   }

   public static void SendSync(string sender, Message message)
   {
      var (subject, cargo) = message;
      foreach (var syncListener in syncListeners[sender])
      {
         syncListener.SyncMessageFrom(sender, subject, cargo);
      }
   }

   public static void Send(string sender, string subject, object cargo) => Send(sender, new Message(subject, cargo));

   public static void SendSync(string sender, string subject, object cargo) => SendSync(sender, new Message(subject, cargo));

   public static void Send(string sender, string subject) => Send(sender, subject, unit);

   public static void SendSync(string sender, string subject) => SendSync(sender, subject, unit);

   [Obsolete("Use RegisterListener(listener, senders)")]
   public static void RegisterListener(string sender, IMessageQueueListener messageQueueListener)
   {
      listeners[sender].Add(messageQueueListener);
   }

   [Obsolete("Use RegisterSyncListener(listener, senders)")]
   public static void RegisterSyncListener(string sender, IMessageQueueSyncListener messageQueueSyncListener)
   {
      syncListeners[sender].Add(messageQueueSyncListener);
   }

   public static void RegisterListener(IMessageQueueListener messageQueueListener, params string[] senders)
   {
      foreach (var sender in senders)
      {
         listeners[sender].Add(messageQueueListener);
      }
   }

   public static void RegisterSyncListener(IMessageQueueSyncListener messageQueueSyncListener, params string[] senders)
   {
      foreach (var sender in senders)
      {
         syncListeners[sender].Add(messageQueueSyncListener);
      }
   }

   [Obsolete("Use UnregisterListener(listener, senders)")]
   public static void UnregisterListener(string sender, IMessageQueueListener messageQueueListener)
   {
      listeners[sender].Remove(messageQueueListener);
   }

   public static void UnregisterSyncListener(string sender, IMessageQueueSyncListener messageQueueSyncListener)
   {
      syncListeners[sender].Remove(messageQueueSyncListener);
   }

   public static void UnregisterListener(IMessageQueueListener messageQueueListener, params string[] senders)
   {
      foreach (var sender in senders)
      {
         listeners[sender].Remove(messageQueueListener);
      }
   }

   public static void UnregisterSyncListener(IMessageQueueSyncListener messageQueueSyncListener, params string[] senders)
   {
      foreach (var sender in senders)
      {
         syncListeners[sender].Remove(messageQueueSyncListener);
      }
   }
}