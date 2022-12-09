namespace Core.Applications.Invokers;

public class Wait
{
   protected WaitType wait;

   public Wait() => wait = WaitType.North;

   public char Next()
   {
      var ch = '?';
      var next = WaitType.North;
      switch (wait)
      {
         case WaitType.North:
            ch = '|';
            next = WaitType.NorthEast;
            break;
         case WaitType.NorthEast:
            ch = '/';
            next = WaitType.East;
            break;
         case WaitType.East:
            ch = '-';
            next = WaitType.SouthEast;
            break;
         case WaitType.SouthEast:
            ch = '\\';
            next = WaitType.South;
            break;
         case WaitType.South:
            ch = '|';
            next = WaitType.SouthWest;
            break;
         case WaitType.SouthWest:
            ch = '/';
            next = WaitType.West;
            break;
         case WaitType.West:
            ch = '-';
            next = WaitType.NorthWest;
            break;
         case WaitType.NorthWest:
            ch = '\\';
            next = WaitType.North;
            break;
      }

      wait = next;
      return ch;
   }
}