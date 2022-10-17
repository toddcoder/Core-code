namespace Core.Enumerables;

public class Int32Range : XRange<int, uint>
{
   public Int32Range(int from, int to, uint distance, bool includeFrom, bool includeTo) : base(from, to, distance, includeFrom, includeTo) { }

   protected override int nextValue(int currentValue) => currentValue + (int)distance;

   protected override int previousValue(int currentValue) => currentValue - (int)distance;

   public override int Compare(int x, int y) => x.CompareTo(y);
}