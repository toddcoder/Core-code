using System;
using Core.Exceptions;

namespace Core.Objects
{
	public static class Assertions
	{
		[Obsolete]
		public static void AssertIsNotNull(this object test, string message)
		{
			if (test == null)
				throw message.Throws();
		}

		public static void AssertIsNotNull(this object test, string message, Exception innerException)
		{
			if (test == null)
				throw message.Throws(innerException);
		}

		public static void AssertIsNotNull(this object test, Func<string> message)
		{
			if (test == null)
				throw message().Throws();
		}

		public static void AssertIsNotNull<TException>(this object test)
			where TException : Exception, new()
		{
			if (test == null)
				throw Throwing.Throws<TException>();
		}

		public static void AssertIsNotNull<TException>(this object test, params object[] parameters)
			where TException : Exception
		{
			if (test == null)
				throw Throwing.Throws<TException>(parameters);
		}

		public static void RejectIfNotNull(this object test, string message)
		{
			if (test != null)
				throw message.Throws();
		}

		public static void RejectIfNotNull(this object test, Func<string> message)
		{
			if (test != null)
				throw message().Throws();
		}

		public static void RejectIfNotNull(this object test, string message, Exception innerException)
		{
			if (test != null)
				throw message.Throws(innerException);
		}

		public static void RejectIfNotNull<TException>(this object test)
			where TException : Exception, new()
		{
			if (test != null)
				throw Throwing.Throws<TException>();
		}

		public static void RejectIfNotNull<TException>(this object test, params object[] parameters)
			where TException : Exception
		{
			if (test != null)
				throw Throwing.Throws<TException>(parameters);
		}

		public static void AssertIsNull(this object test, string message)
		{
			if (test != null)
				throw message.Throws();
		}

		public static void AssertIsNull(this object test, Func<string> message)
		{
			if (test != null)
				throw message().Throws();
		}

		public static void AssertIsNull(this object test, string message, Exception innerException)
		{
			if (test != null)
				throw message.Throws(innerException);
		}

		public static void AssertIsNull<TException>(this object test)
			where TException : Exception, new()
		{
			if (test != null)
				throw Throwing.Throws<TException>();
		}

		public static void AssertIsNull<TException>(this object test, params object[] parameters)
			where TException : Exception
		{
			if (test != null)
				throw Throwing.Throws<TException>(parameters);
		}

		public static void RejectIfNull(this object test, string message)
		{
			if (test == null)
				throw message.Throws();
		}

		public static void RejectIfNull(this object test, Func<string> message)
		{
			if (test == null)
				throw message().Throws();
		}

		public static void RejectIfNull(this object test, string message, Exception innerException)
		{
			if (test == null)
				throw message.Throws(innerException);
		}

		public static void RejectIfNull<TException>(this object test)
			where TException : Exception, new()
		{
			if (test == null)
				throw Throwing.Throws<TException>();
		}

		public static void RejectIfNull<TException>(this object test, params object[] parameters)
			where TException : Exception
		{
			if (test == null)
				throw Throwing.Throws<TException>(parameters);
		}
	}
}