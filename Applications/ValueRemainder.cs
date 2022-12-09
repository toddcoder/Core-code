namespace Core.Applications;

public class ValueRemainder
{
   public ValueRemainder(object value, string remainder)
   {
      Value = value;
      Remainder = remainder;
   }

   public object Value { get; }

   public string Remainder { get; }

   public void Deconstruct(out object value, out string remainder)
   {
      value = Value;
      remainder = Remainder;
   }
}