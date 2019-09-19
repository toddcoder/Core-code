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

namespace Core.Assertions.Objects
{
	public class ObjectAssertion : IAssertion<object>
	{
		public static implicit operator bool(ObjectAssertion assertion) => assertion.BeTrue();

		protected object obj;
		protected List<Constraint> constraints;
		protected bool not;

		public ObjectAssertion(object obj)
		{
			this.obj = obj;
			constraints = new List<Constraint>();
			not = false;
		}

		public ObjectAssertion Not
		{
			get
			{
				not = true;
				return this;
			}
		}

		protected ObjectAssertion add(object other, Func<object, bool> constraintFunction, string message)
		{
			switch (other)
			{
				case null:
					constraints.Add(Constraint.Failing("RHS must be non-null"));
					break;
				default:
					constraints.Add(new Constraint(() => constraintFunction(other), message, not));
					break;
			}

			not = false;
			return this;
		}

		protected ObjectAssertion add(Func<bool> constraintFunction, string message)
		{
			constraints.Add(new Constraint(constraintFunction, message, not));
			not = false;

			return this;
		}

		public ObjectAssertion Equal(object other)
		{
			return add(other, o => obj.Equals(o), $"{obj} must $not equal {other}");
		}

		public ObjectAssertion BeNull()
		{
			return add(() => obj == null, "This value must $not be null");
		}

		public ObjectAssertion BeOfType(Type type)
		{
			return add(() => obj.GetType() == type, $"{obj} must $not be of type {type}");
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

		public object Ensure()
		{
			Assert();
			return obj;
		}

		public object Ensure(string message)
		{
			Assert(message);
			return obj;
		}

		public object Ensure(Func<string> messageFunc)
		{
			Assert(messageFunc);
			return obj;
		}

		public object Ensure<TException>(params object[] args) where TException : Exception
		{
			Assert<TException>(args);
			return obj;
		}

		public T Ensure<T>() => (T)Ensure();

		public TResult Ensure<TResult>(string message) => (TResult)Ensure(message);

		public TResult Ensure<TResult>(Func<string> messageFunc) => (TResult)Ensure(messageFunc);

		public TResult Ensure<TException, TResult>(params object[] args) where TException : Exception
		{
			return (TResult)Ensure<TException>(args);
		}

		public IResult<object> Try()
		{
			if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
			{
				return constraint.Message.Failure<object>();
			}
			else
			{
				return obj.Success();
			}
		}

		public async Task<ICompletion<object>> TryAsync(CancellationToken token) => await runAsync(t =>
		{
			if (constraints.FirstOrNone(c => !c.IsTrue()).If(out var constraint))
			{
				return constraint.Message.Interrupted<object>();
			}
			else
			{
				return obj.Completed(t);
			}
		}, token);
	}
}