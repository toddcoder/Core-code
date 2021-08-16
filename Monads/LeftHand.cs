namespace Core.Monads
{
   public class LeftHand<T>
   {
      public LeftHand(T left)
      {
         Left = left;
      }

      public T Left { get; }
   }
}