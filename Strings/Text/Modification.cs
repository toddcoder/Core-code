namespace Core.Strings.Text
{
   internal class Modification
   {
      public Modification(string[] rawData)
      {
         RawData = rawData;
      }

      public int[] HashedItems { get; set; }

      public string[] RawData { get; }

      public bool[] Modifications { get; set; }

      public string[] Items { get; set; }
   }
}