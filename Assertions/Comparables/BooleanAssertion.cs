using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using System.Linq;
using Core.Enumerables;
using Core.Exceptions;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.AttemptFunctions;

namespace Core.Assertions.Comparables
{
	public class BooleanAssertion : IAssertion<bool>
	{
		public static implicit operator bool(BooleanAssertion assertion) => assertion.BeTrue();

		protected bool boolean;
		protected List<Constraint> constraints;
		protected bool not;

		public BooleanAssertion(bool boolean)
		{
			this.boolean = boolean;
			constraints = new List<Constraint>();
			not = false;
		}

		public bool Boolean => boolean;

		public BooleanAssertion Not
		{
			get
			{
				not = true;
				return this;
			}
		}

		protected BooleanAssertion add(object obj, Func<bool, bool> constraintFunction, string message)
		{
			switch (obj)
			{
				case null:
					constraints.Add(Constraint.Failing("RHS must be non-null"));
					break;
				case bool otherBoolean:
					constraints.Add(new Constraint(() => constraintFunction(otherBoolean), message, not));
					break;
				default:
					constraints.Add(Constraint.Failing($"{obj} must be comparable"));
					break;
			}

			not = false;
			return this;
		}

		protected BooleanAssertion add(Func<bool> constraintFunction, string message)
		{
			constraints.Add(new Constraint(constraintFunction, message, not));
			not = false;

			return this;
		}

		public BooleanAssertion Be()
		{
			return add(() => boolean, $"{boolean} must $not be true");
		}

		public BooleanAssertion And(bool test)
		{
			return add(() => boolean && test, $"{boolean} and $not {test}");
		}

		public BooleanAssertion Or(bool test)
		{
			return add(() => boolean || test, $"{boolean} or $not {test}");
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

		public bool Ensure()
		{
			Assert();
			return boolean;
		}

		public bool Ensure(string message)
		{
			Assert(message);
			return boolean;
		}

		public bool Ensure(Func<string> messageFunc)
		{
			Assert(messageFunc);
			return boolean;
		}

		public bool Ensure<TException>(params object[] args) where TException : Exception
		{
			Assert<TException>(args);
			return boolean;
		}

		public TResult Ensure<TResult>()
		{
			Assert();
			var converter = TypeDescriptor.GetConverter(typeof(bool));
			return (TResult)converter.ConvertTo(boolean, typeof(TResult));
		}

		public TResult Ensure<TResult>(string message)
		{
			Assert(message);
			var converter = TypeDescriptor.GetConverter(typeof(bool));
			return (TResult)converter.ConvertTo(boolean, typeof(TResult));
		}

		public TResult Ensure<TResult>(Func<string> messageFunc)
		{
			Assert(messageFunc);
			var converter = TypeDescriptor.GetConverter(typeof(bool));
			return (TResult)converter.ConvertTo(boolean, typeof(TResult));
		}

		public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
		{
			Assert<TException>(args);
			var converter = TypeDescriptor.GetConverter(typeof(bool));
			return (TResult)converter.ConvertTo(boolean, typeof(TResult));
		}

		public IResult<bool> Try()
		{
			if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
			{
				return constraint.Message.Failure<bool>();
			}
			else
			{
				return boolean.Success();
			}
		}

		public async Task<ICompletion<bool>> TryAsync(CancellationToken token) => await runAsync(t =>
		{
			if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
			{
				return constraint.Message.Interrupted<bool>();
			}
			else
			{
				return boolean.Completed(t);
			}
		}, token);
	}
}