namespace Core.Monads
{
   public class NilWithMessage : Nil
   {
      protected string message;

      public NilWithMessage(string message)
      {
         this.message = message;
      }

      public string Message => message;

      public bool Equals(NilWithMessage nilWithMessage) => message == nilWithMessage.message;

      public override bool Equals(object obj) => obj is NilWithMessage other && Equals(other);

      public override int GetHashCode() => base.GetHashCode() ^ message.GetHashCode();

      public static bool operator ==(NilWithMessage left, NilWithMessage right) => Equals(left, right);

      public static bool operator !=(NilWithMessage left, NilWithMessage right) => !Equals(left, right);
   }
}