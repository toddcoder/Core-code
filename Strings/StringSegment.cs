namespace Core.Strings;

public class StringSegment
{
   public static implicit operator string(StringSegment segment) => segment.String;

   public static implicit operator bool(StringSegment segment) => segment.StartIndex > -1 && segment.StopIndex > -1;

   protected string str;
   protected int startIndex;
   protected int stopIndex;

   public StringSegment(string str, int startIndex, int stopIndex)
   {
      this.str = str;
      this.startIndex = startIndex;
      this.stopIndex = stopIndex;
   }

   public StringSegment()
   {
      str = string.Empty;
      startIndex = -1;
      stopIndex = -1;
   }

   public string String => str;

   public int StartIndex => startIndex;

   public int StopIndex => stopIndex;

   public override string ToString() => str;
}