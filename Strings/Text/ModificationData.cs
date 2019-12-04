namespace Core.Strings.Text
{
   public class ModificationData
   {
      public ModificationData(string[] rawData)
      {
         RawData = rawData;
      }

      public int[] HashedItems { get; set; }

      public string[] RawData { get; }

      public bool[] Modifications { get; set; }

      public string[] Items { get; set; }
   }
}