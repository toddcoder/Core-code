using System;
using System.Linq;
using System.Reflection;
using Core.Collections;

namespace Core.Objects
{
	public class Equatable<T> : IEquatable<T>
	{
		static Hash<string, object> getValues(T obj, string[] signatures)
		{
			var fieldValues = typeof(T).GetRuntimeFields()
				.Where(fi => signatures.Contains(fi.Name))
				.Select(fi => (signature: fi.Name, value: fi.GetValue(obj)))
				.ToHash(t => t.signature, t => t.value);
			var propertyValues = typeof(T).GetRuntimeProperties()
				.Where(pi => signatures.Contains(pi.Name))
				.Select(pi => (signature: pi.Name, value: pi.GetValue(obj)))
				.ToHash(t => t.signature, t => t.value);

			return fieldValues.Merge(propertyValues);
		}

		T obj;
		string[] signatures;

		public Equatable(T obj, params string[] signatures)
		{
			this.obj = obj;
			this.signatures = signatures;
		}

		public bool Equals(T other)
		{
			if (ReferenceEquals(null, other))
         {
            return false;
         }
         else if (ReferenceEquals(obj, other))
         {
            return true;
         }
         else
			{
				var values = getValues(obj, signatures);
				var otherValues = getValues(other, signatures);
				return signatures
					.Select(signature => values[signature].Equals(otherValues[signature]))
					.All(b => b);
			}
		}

		public override bool Equals(object other) => other is T tOther && Equals(tOther);

		public override int GetHashCode()
		{
			unchecked
			{
				var values = getValues(obj, signatures);
				return values.Values
					.Select(value => value?.GetHashCode() ?? 0)
					.Aggregate(397, (current, value) => current * 397 ^ value);
			}
		}
	}
}