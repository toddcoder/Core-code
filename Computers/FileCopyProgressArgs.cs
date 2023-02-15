using System;

namespace Core.Computers;

public class FileCopyProgressArgs : EventArgs
{
   public FileCopyProgressArgs(double percentage)
   {
      Percentage = percentage;
   }

   public double Percentage { get; }
}