using System;

namespace Core.Computers;

public class FileCopyFinishedArgs
{
   public FileCopyFinishedArgs(long bytesTransferred, TimeSpan elapsedTime)
   {
      BytesTransferred = bytesTransferred;
      ElapsedTime = elapsedTime;
   }

   public long BytesTransferred { get; }

   public TimeSpan ElapsedTime { get; }
}