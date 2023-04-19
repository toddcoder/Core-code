using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Collections;

namespace Core.Applications.Messaging;

public static class MessageQueue
{
   private static AutoStringHash<List<IMessageQueueListener>> listeners;

   static MessageQueue()
   {
      listeners = new AutoStringHash<List<IMessageQueueListener>>(true, _ => new List<IMessageQueueListener>(), true);
   }

   public static void Send(string sender, Message message)
   {
      var (subject, cargo) = message;
      foreach (var messageListener in listeners[sender])
      {
         Task.Run(() => messageListener.MessageFrom(sender, subject, cargo));
      }
   }

   public static void Send(string sender, string subject, object cargo) => Send(sender, new Message(subject, cargo));

   public static void RegisterListener(string sender, IMessageQueueListener messageQueueListener) => listeners[sender].Add(messageQueueListener);

   public static void RegisterListener(IMessageQueueListener messageQueueListener, params string[] senders)
   {
      foreach (var sender in senders)
      {
         RegisterListener(sender, messageQueueListener);
      }
   }

   public static void UnregisterListener(string sender, IMessageQueueListener messageQueueListener) => listeners[sender].Remove(messageQueueListener);

   public static void UnregisterListener(IMessageQueueListener messageQueueListener, params string[] senders)
   {
      foreach (var sender in senders)
      {
         UnregisterListener(sender, messageQueueListener);
      }
   }
}