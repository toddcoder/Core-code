namespace Core.Objects
{
   public interface ITryAsync<TTry, TAsync>
   {
      TTry TryTo { get; }

      TAsync Async { get; }
   }
}