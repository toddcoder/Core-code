using System.ComponentModel;
using System.Runtime.InteropServices;
using Core.Monads;

namespace Core.Applications
{
	public static class ConsoleFunctions
	{
		const uint ERROR_ACCESS_DENIED = 5;

		static string errorMessage() => new Win32Exception(Marshal.GetLastWin32Error()).Message;

		public static IResult<Unit> consoleNew(bool alwaysCreateNewConsole = true)
		{
			if (alwaysCreateNewConsole ? !Kernel32.consoleAllocate() : !Kernel32.consoleAttach())
         {
            return
               from outStream in Kernel32.initializeOutStream()
               from inStream in Kernel32.initializeInStream()
               select Unit.Value;
         }
         else
         {
            return errorMessage().Failure<Unit>();
         }
      }
	}
}