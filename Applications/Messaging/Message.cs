namespace Core.Applications.Messaging;

public class Message
{
   public Message(string subject, object cargo)
   {
      Subject = subject;
      Cargo = cargo;
   }

   public string Subject { get; }

   public object Cargo { get; }

   public void Deconstruct(out string subject, out object cargo)
   {
      subject = Subject;
      cargo = Cargo;
   }
}