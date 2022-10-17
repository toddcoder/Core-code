namespace Core.Enumerables;

public class IntIterator : Iterator<int>
{
   protected int increment;

   public IntIterator(int seed) : base(seed)
   {
      increment = 1;
      next = i => i + increment;
   }
}