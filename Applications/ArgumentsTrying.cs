using Core.Assertions;
using Core.Monads;

namespace Core.Applications
{
	public class ArgumentsTrying
	{
		Arguments arguments;

		public ArgumentsTrying(Arguments arguments) => this.arguments = arguments;

		public IResult<Argument> this[int index]
      {
         get => index.Must().BeBetween(0).Until(arguments.Count).Try(() => $"Index {index} out of range").Map(i => arguments[i]);
      }

      public IResult<Unit> AssertCount(int exactCount)
      {
         return arguments.Count.Must().Equal(exactCount).Try(() => $"Expected exact count of {exactCount}").Unit;
      }

		public IResult<Unit> AssertCount(int minimumCount, int maximumCount)
      {
         return arguments.Count.Must()
            .BeBetween(minimumCount).Until(arguments.Count)
            .Try(() => $"Count must between {minimumCount} and {maximumCount}--found {arguments.Count}")
            .Unit;
      }

		public IResult<Unit> AssertMinimumCount(int minimumCount)
      {
         return arguments.Count.Must().BeGreaterThanOrEqual(minimumCount)
            .Try(() => $"Count must be at least {minimumCount}--found {arguments.Count}")
            .Unit;
      }

		public IResult<Unit> AssertMaximumCount(int maximumCount)
      {
         return arguments.Count.Must().BeLessThanOrEqual(maximumCount)
            .Try(() => $"Count must be at most {maximumCount}--found {arguments.Count}")
            .Unit;
      }
	}
}