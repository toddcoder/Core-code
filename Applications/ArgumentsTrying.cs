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
         get => index.MustAs(nameof(index)).BeBetween(0).Until(arguments.Count).Try(() => "Index $name out of range").Map(i => arguments[i]);
      }

      public IResult<Unit> AssertCount(int exactCount)
      {
         return arguments.Count.MustAs("arguments count").Equal(exactCount).Try(() => $"Expected exact $name of {exactCount}").Unit;
      }

		public IResult<Unit> AssertCount(int minimumCount, int maximumCount)
      {
         return arguments.Count.MustAs("arguments count")
            .BeBetween(minimumCount).Until(arguments.Count)
            .Try(() => $"$name must between {minimumCount} and {maximumCount}--found {arguments.Count}")
            .Unit;
      }

		public IResult<Unit> AssertMinimumCount(int minimumCount)
      {
         return arguments.Count.MustAs("arguments count").BeGreaterThanOrEqual(minimumCount)
            .Try(() => $"$name must be at least {minimumCount}--found {arguments.Count}")
            .Unit;
      }

		public IResult<Unit> AssertMaximumCount(int maximumCount)
      {
         return arguments.Count.MustAs("arguments count").BeLessThanOrEqual(maximumCount)
            .Try(() => $"$name must be at most {maximumCount}--found {arguments.Count}")
            .Unit;
      }
	}
}