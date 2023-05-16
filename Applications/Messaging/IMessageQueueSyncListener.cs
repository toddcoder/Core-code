namespace Core.Applications.Messaging;

public interface IMessageQueueSyncListener
{
   string Listener { get; }

   void SyncMessageFrom(string sender, string subject, object cargo);
}