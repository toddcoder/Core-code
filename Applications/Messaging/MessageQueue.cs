using System.Collections.Generic;
using Core.Collections;
using Core.DataStructures;
using Core.Monads;

namespace Core.Applications.Messaging;

public static class MessageQueue
{
   private static AutoStringHash<MaybeQueue<Message>> queues;
   private static AutoStringHash<List<IMessageQueueListener>> listeners;

   static MessageQueue()
   {
      queues = new AutoStringHash<MaybeQueue<Message>>(true, _ => new MaybeQueue<Message>(), true);
      listeners = new AutoStringHash<List<IMessageQueueListener>>(true, _ => new List<IMessageQueueListener>(), true);
   }

   public static void Enqueue(string sender, Message message)
   {
      queues[sender].Enqueue(message);
      foreach (var messageListener in listeners[sender])
      {
         messageListener.MessageFrom(sender);
      }
   }

   public static void Enqueue(string sender, string subject, object cargo) => Enqueue(sender, new Message(subject, cargo));

   public static Maybe<Message> Dequeue(string sender) => queues.Maybe[sender].Map(q => q.Dequeue());

   public static Maybe<Message> Peek(string sender) => queues.Maybe[sender].Map(q => q.Peek());

   public static void RegisterListener(string sender, IMessageQueueListener messageQueueListener) => listeners[sender].Add(messageQueueListener);

   public static void UnregisterListener(string sender, IMessageQueueListener messageQueueListener) => listeners[sender].Remove(messageQueueListener);
}