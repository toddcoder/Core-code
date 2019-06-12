using System;

namespace Core.WinForms.Controls
{
   public class ProgressEventArgs : EventArgs
   {
      public ProgressEventArgs(TimeSpan elapsed) => Elapsed = elapsed;

      public TimeSpan Elapsed { get; }
   }
}