using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using Core.Exceptions;

namespace Core.Computers
{
	public class Impersonator : IDisposable
	{
		sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			[DllImport("kernel32.dll"), SuppressUnmanagedCodeSecurity]
			static extern bool CloseHandle(IntPtr handle);

			public SafeTokenHandle() : base(true) { }

			protected override bool ReleaseHandle() => CloseHandle(handle);
		}

		const int LOGON32_LOGON_INTERACTIVE = 2;
		const int LOGON32_LOGON_BATCH = 4;

		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern bool LogonUser(string userName, string domain, string password, int logonType, int logonProvider,
			out SafeTokenHandle handle);

		SafeTokenHandle handle;
		WindowsImpersonationContext context;

		public Impersonator(string domain, string userName, string password, bool service)
		{
			var type = service ? LOGON32_LOGON_BATCH : LOGON32_LOGON_INTERACTIVE;
			var ok = LogonUser(userName, domain, password, type, 0, out handle);
			if (!ok)
			{
				var error = Marshal.GetLastWin32Error();
				throw $"Couldn't impersonate user: error code {error}".Throws();
			}
			else
         {
            context = WindowsIdentity.Impersonate(handle.DangerousGetHandle());
         }
      }

		public Impersonator(ImpersonatorArguments arguments)
			: this(arguments.Domain, arguments.UserName, arguments.Password, arguments.Service) { }

		public void Dispose()
		{
			context?.Dispose();
			handle?.Dispose();
		}
	}
}