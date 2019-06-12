using Core.Monads;
using Core.Numbers;

namespace Core.Applications
{
	public class ArgumentsTrying
	{
		Arguments arguments;

		public ArgumentsTrying(Arguments arguments) => this.arguments = arguments;

		public IResult<Argument> this[int index] => AttemptFunctions.assert(() => index.Between(0).Until(arguments.Count),
			() => arguments[index], () => $"Index {index} out of range");

		public IResult<Unit> AssertCount(int exactCount)
		{
			return AttemptFunctions.assert(arguments.Count == exactCount, () => Unit.Value,
				() => $"Expected exact count of {exactCount}");
		}

		public IResult<Unit> AssertCount(int minimumCount, int maximumCount)
		{
			return AttemptFunctions.assert(arguments.Count.Between(0).Until(arguments.Count), () => Unit.Value,
				() => $"Count must between {minimumCount} and {maximumCount}--found {arguments.Count}");
		}

		public IResult<Unit> AssertMinimumCount(int minimumCount)
		{
			return AttemptFunctions.assert(arguments.Count >= minimumCount, () => Unit.Value,
				() => $"Count must be at least {minimumCount}--found {arguments.Count}");
		}

		public IResult<Unit> AssertMaximumCount(int maximumCount)
		{
			return AttemptFunctions.assert(arguments.Count <= maximumCount, () => Unit.Value,
				() => $"Count must be at most {maximumCount}--found {arguments.Count}");
		}
	}
}