using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Core.Threading
{
   public class SafeThreadHandle : SafeHandleMinusOneIsInvalid
   {
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success),
       DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
      protected static extern bool CloseHandle(IntPtr handle);

      public SafeThreadHandle() : base(true)
      {
      }

      public SafeThreadHandle(bool ownsHandle) : base(ownsHandle)
      {
      }

      protected override bool ReleaseHandle() => CloseHandle(handle);
   }
}