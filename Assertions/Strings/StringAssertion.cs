using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Exceptions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Strings
{
	public class StringAssertion : IAssertion<string>
	{
		public static implicit operator bool(StringAssertion assertion) => assertion.BeTrue();

      public static bool operator &(StringAssertion x, ICanBeTrue y) => and(x, y);

      public static bool operator |(StringAssertion x, ICanBeTrue y) => or(x, y);

      protected static bool inList(string subject, object[] strings) => strings.Any(s => subject.CompareTo(s) == 0);

		protected string subject;
		protected List<Constraint> constraints;
		protected bool not;

		public StringAssertion(string subject)
		{
			this.subject = subject;
			constraints = new List<Constraint>();
			not = false;
		}

		public string Subject => subject;

		public StringAssertion Not
		{
			get
			{
				not = true;
				return this;
			}
		}

		protected StringAssertion add(object other, Func<string, bool> constraintFunction, string message)
		{
			switch (other)
			{
				case null:
					constraints.Add(Constraint.Failing("RHS must be non-null"));
					break;
				case string otherString:
					constraints.Add(new Constraint(() => constraintFunction(otherString), message, not));
					break;
				default:
					constraints.Add(Constraint.Failing($"{other} must be string"));
					break;
			}

			not = false;
			return this;
		}

		protected StringAssertion add(Func<bool> constraintFunction, string message)
		{
			constraints.Add(new Constraint(constraintFunction, message, not));
			not = false;

			return this;
		}

		public StringAssertion Equal(object obj)
		{
			return add(obj, s => subject.CompareTo(s) == 0, $"{subject} must $not equal \"{obj}\"");
		}

		public StringAssertion BeGreaterThan(object obj)
		{
			return add(obj, s => subject.CompareTo(s) > 0, $"{subject} must $not be > \"{obj}\"");
		}

		public StringAssertion BeGreaterThanOrEqual(object obj)
		{
			return add(obj, s => subject.CompareTo(s) >= 0, $"{subject} must $not be >= \"{obj}\"");
		}

		public StringAssertion BeLessThan(object obj)
		{
			return add(obj, s => subject.CompareTo(s) < 0, $"{subject} must $not be < \"{obj}\"");
		}

		public StringAssertion BeLessThanOrEqual(object obj)
		{
			return add(obj, s => subject.CompareTo(s) <= 0, $"{subject} must $not be <= \"{obj}\"");
		}

		public StringAssertion BeNull()
		{
			return add(() => subject == null, "This value must $not be null");
		}

		public StringAssertion BeEmpty()
		{
			return add(() => subject == string.Empty, "This value must $not be empty");
		}

		public StringAssertion BeNullOrEmpty()
		{
			return add(() => string.IsNullOrEmpty(subject), "This value must $not be null or empty");
		}

		public StringAssertion BeNullOrWhiteSpace()
		{
			return add(() => string.IsNullOrWhiteSpace(subject), "This value must $not be null or white-space");
		}

		public StringAssertion HaveLengthOf(int length)
		{
			return add(() => subject.Length >= length, $"This value must $not have a length >= {length}");
		}

      public string Value => subject;

      public IEnumerable<Constraint> Constraints => constraints;

      public bool BeTrue() => beTrue(this);

		public void Assert() => assert(this);

      public void Assert(string message) => assert(this, message);

      public void Assert(Func<string> messageFunc) => assert(this, messageFunc);

      public void Assert<TException>(params object[] args) where TException : Exception => assert<TException, string>(this, args);

      public string Ensure() => ensure(this);

      public string Ensure(string message) => ensure(this, message);

      public string Ensure(Func<string> messageFunc) => ensure(this, messageFunc);

      public string Ensure<TException>(params object[] args) where TException : Exception => ensure<TException, string>(this, args);

      public TResult Ensure<TResult>() => throw "Can't convert string to another type".Throws();

		public TResult Ensure<TResult>(string message) => Ensure<TResult>();

		public TResult Ensure<TResult>(Func<string> messageFunc) => Ensure<TResult>();

		public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception => Ensure<TResult>();

		public IResult<string> Try() => @try(this);

      public IResult<string> Try(string message) => @try(this, message);

      public IResult<string> Try(Func<string> messageFunc) => @try(this, messageFunc);

      public async Task<ICompletion<string>> TryAsync(CancellationToken token) => await tryAsync(this, token);

      public async Task<ICompletion<string>> TryAsync(string message, CancellationToken token) => await tryAsync(this, message, token);

      public async Task<ICompletion<string>> TryAsync(Func<string> messageFunc, CancellationToken token) => await tryAsync(this, messageFunc, token);
   }
}