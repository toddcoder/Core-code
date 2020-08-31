using System;

namespace Core.Strings
{
   public class SegmentArgs : EventArgs
   {
      public SegmentArgs(string segment, int index)
      {
         Segment = segment;
         Index = index;
      }

      public string Segment { get; }

      public int Index { get; }
   }
}