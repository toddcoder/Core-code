namespace Core.Applications.Messaging;

public interface IMessageQueueListener
{
   string Listener { get; }

   void MessageFrom(string sender);
}