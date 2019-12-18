using System;
using Core.Computers;

namespace Core.Zip
{
   public class ZipEventArgs : EventArgs
   {
      public ZipEventArgs(FileName file)
      {
         File = file;
         ZipEventCancel = ZipEventCancel.None;
      }

      public FileName File { get; }

      public ZipEventCancel ZipEventCancel { get; set; }
   }
}