using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Exceptions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.AttemptFunctions;

namespace Core.Assertions.Strings
{
	public class StringAssertion : IAssertion<string>
	{
		public static implicit operator bool(StringAssertion assertion) => assertion.BeTrue();

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

		public bool BeTrue() => constraints.All(constraint => constraint.IsTrue());

		public void Assert()
		{
			if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
			{
				throw constraint.Message.Throws();
			}
		}

		public void Assert(string message)
		{
			if (constraints.Any(c => !c.IsTrue()))
			{
				throw message.Throws();
			}
		}

		public void Assert(Func<string> messageFunc)
		{
			if (constraints.Any(c => !c.IsTrue()))
			{
				throw messageFunc().Throws();
			}
		}

		public void Assert<TException>(params object[] args) where TException : Exception
		{
			if (constraints.Any(c => !c.IsTrue()))
			{
				throw getException<TException>(args);
			}
		}

		public string Ensure()
		{
			Assert();
			return subject;
		}

		public string Ensure(string message)
		{
			Assert(message);
			return subject;
		}

		public string Ensure(Func<string> messageFunc)
		{
			Assert(messageFunc);
			return subject;
		}

		public string Ensure<TException>(params object[] args) where TException : Exception
		{
			Assert<TException>(args);
			return subject;
		}

		public TResult Ensure<TResult>() => throw "Can't convert string to another type".Throws();

		public TResult Ensure<TResult>(string message) => Ensure<TResult>();

		public TResult Ensure<TResult>(Func<string> messageFunc) => Ensure<TResult>();

		public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception => Ensure<TResult>();

		public IResult<string> Try()
		{
			if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
			{
				return constraint.Message.Failure<string>();
			}
			else
			{
				return subject.Success();
			}
		}

		public async Task<ICompletion<string>> TryAsync(CancellationToken token) => await runAsync(t =>
		{
			if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
			{
				return constraint.Message.Interrupted<string>();
			}
			else
			{
				return subject.Completed(t);
			}
		}, token);
	}
}