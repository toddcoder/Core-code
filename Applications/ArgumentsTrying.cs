using Core.Assertions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Applications
{
   public class ArgumentsTrying
   {
      Arguments arguments;

      public ArgumentsTrying(Arguments arguments) => this.arguments = arguments;

      public IResult<Argument> this[int index]
      {
         get => assert(() => index).Must().BeBetween(0).Until(arguments.Count).OrFailure(() => "Index $name out of range").Map(i => arguments[i]);
      }

      public IResult<Unit> AssertCount(int exactCount)
      {
         return assert(() => arguments.Count).Must().Equal(exactCount).OrFailure(() => $"Expected exact $name of {exactCount}").Unit;
      }

      public IResult<Unit> AssertCount(int minimumCount, int maximumCount)
      {
         return assert(() => arguments.Count).Must()
            .BeBetween(minimumCount).Until(arguments.Count)
            .OrFailure(() => $"$name must between {minimumCount} and {maximumCount}--found {arguments.Count}")
            .Unit;
      }

      public IResult<Unit> AssertMinimumCount(int minimumCount)
      {
         return assert(() => arguments.Count).Must().BeGreaterThanOrEqual(minimumCount)
            .OrFailure(() => $"$name must be at least {minimumCount}--found {arguments.Count}")
            .Unit;
      }

      public IResult<Unit> AssertMaximumCount(int maximumCount)
      {
         return assert(() => arguments.Count).Must().BeLessThanOrEqual(maximumCount)
            .OrFailure(() => $"$name must be at most {maximumCount}--found {arguments.Count}")
            .Unit;
      }
   }
}