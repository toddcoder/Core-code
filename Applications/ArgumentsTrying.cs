using Core.Assertions;
using Core.Monads;

namespace Core.Applications;

public class ArgumentsTrying
{
   protected Arguments arguments;

   public ArgumentsTrying(Arguments arguments) => this.arguments = arguments;

   public Result<Argument> this[int index]
   {
      get => index.Must().BeBetween(0).Until(arguments.Count).OrFailure(() => "Index $name out of range").Map(i => arguments[i]);
   }

   public Result<Unit> AssertCount(int exactCount)
   {
      return arguments.Count.Must().Equal(exactCount).OrFailure(() => $"Expected exact $name of {exactCount}").Unit;
   }

   public Result<Unit> AssertCount(int minimumCount, int maximumCount)
   {
      return arguments.Count.Must()
         .BeBetween(minimumCount).Until(arguments.Count)
         .OrFailure(() => $"$name must between {minimumCount} and {maximumCount}--found {arguments.Count}")
         .Unit;
   }

   public Result<Unit> AssertMinimumCount(int minimumCount)
   {
      return arguments.Count.Must().BeGreaterThanOrEqual(minimumCount)
         .OrFailure(() => $"$name must be at least {minimumCount}--found {arguments.Count}")
         .Unit;
   }

   public Result<Unit> AssertMaximumCount(int maximumCount)
   {
      return arguments.Count.Must().BeLessThanOrEqual(maximumCount)
         .OrFailure(() => $"$name must be at most {maximumCount}--found {arguments.Count}")
         .Unit;
   }
}