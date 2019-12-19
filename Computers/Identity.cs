using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Core.Assertions;
using Core.Monads;
using Core.RegularExpressions;
using static Core.Monads.MonadFunctions;

namespace Core.Computers
{
	public class Identity : IDisposable
	{
		public class IdentityException : ApplicationException
		{
			const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
			const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
			const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
			const int MESSAGE_SIZE = 255;
			const int MESSAGE_FLAGS = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM |
				FORMAT_MESSAGE_IGNORE_INSERTS;

			[DllImport("kernel32.dll")]
			static extern int FormatMessage(int flags, out IntPtr source, int messageID, int languageID, out string buffer, int size,
				out IntPtr arguments);

			static string getErrorMessage(int errorCode)
			{
				var result = FormatMessage(MESSAGE_FLAGS, out _, errorCode, 0, out var messageBuffer, MESSAGE_SIZE, out _);
				return result == 0 ? $"Error code: {errorCode} (Couldn't format message)" : messageBuffer;
			}

			static string errorMessage(string domain, string userName)
			{
				var errorCode = Marshal.GetLastWin32Error();
				return $"Windows Identity couldn't be assumed for {domain}\\{userName} (Win32 code = {errorCode}," +
					$" message=\"{getErrorMessage(errorCode)}\")";
			}

			public IdentityException(string domain, string userName) : base(errorMessage(domain, userName)) { }
		}

		public enum IdentityType
		{
			Interactive = 2,
			Network = 3,
			Batch = 4,
			Service = 5,
			Unlock = 7,
			NetworkClearText = 8,
			NewCredentials = 9
		}

		public enum IdentityProvider
		{
			Default = 0,
			NT35 = 1,
			NT40 = 2,
			NT50 = 3
		}

		const int SECURITY_IMPERSONATION = 2;

		[DllImport("advapi32.dll", SetLastError = true)]
		static extern int LogonUser(string userName, string domain, string password, int logonType, int logonProvider,
			out IntPtr token);

		[DllImport("advapi32.dll", SetLastError = true)]
		static extern int DuplicateToken(IntPtr existingTokenHandle, int securityImpersonationLevel,
			out IntPtr duplicateTokenHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool CloseHandle(IntPtr handle);

		string userName;
		string domain;
		IntPtr tokenHandle;
		IntPtr dupTokenHandle;
		bool impersonating;
		IMaybe<WindowsImpersonationContext> impersonatedUser;

		public Identity()
		{
			Type = IdentityType.Interactive;
			Provider = IdentityProvider.Default;
		}

		public string Domain
		{
			get => domain;
			set => domain = value;
		}

		public string UserName
		{
			get => userName;
			set
			{
				var matcher = new Matcher();
				if (matcher.IsMatch(value, @"^ /([/w '-']+) '\' /([/w '-']+) $"))
				{
					domain = matcher[0, 1];
					userName = matcher[0, 2];
				}
				else
            {
               userName = value;
            }
         }
		}

		public string Password { get; set; }

		public IdentityType Type { get; set; }

		public IdentityProvider Provider { get; set; }

		public bool Impersonating
		{
			get => impersonating;
			set
			{
				if (value)
            {
               impersonate();
            }
            else
            {
               unimpersonate();
            }

            impersonating = value;
			}
		}

		void impersonate()
		{
         UserName.MustAs(nameof(UserName)).Not.BeNullOrEmpty().Assert();
         Domain.MustAs(nameof(Domain)).Not.BeNullOrEmpty().Assert();
         Password.MustAs(nameof(Password)).Not.BeNullOrEmpty().Assert();

			if (LogonUser(UserName, Domain, Password, (int)Type, (int)Provider, out tokenHandle) != 0)
         {
            throw new IdentityException(Domain, UserName);
         }
         else if (DuplicateToken(tokenHandle, SECURITY_IMPERSONATION, out dupTokenHandle) == 0)
			{
				CloseHandle(tokenHandle);
				throw new IdentityException(Domain, UserName);
			}
			else
			{
				var identity = new WindowsIdentity(dupTokenHandle);
				impersonatedUser = getContext(identity);
			}
		}

		static IMaybe<WindowsImpersonationContext> getContext(WindowsIdentity identity) => identity.Impersonate().Some();

		void unimpersonate()
		{
			if (impersonatedUser.If(out var iu) && impersonating)
			{
				iu.Undo();

				if (tokenHandle != IntPtr.Zero)
            {
               CloseHandle(tokenHandle);
            }

            if (dupTokenHandle != IntPtr.Zero)
            {
               CloseHandle(dupTokenHandle);
            }

            impersonatedUser = none<WindowsImpersonationContext>();
			}

			impersonating = false;
		}

		void dispose()
		{
			if (impersonating)
         {
            unimpersonate();
         }
      }

		public void Dispose()
		{
			dispose();
			GC.SuppressFinalize(this);
		}

		~Identity() => dispose();
	}
}