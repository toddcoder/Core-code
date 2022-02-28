using Core.Monads;

namespace Core.Git
{
   public class GitTry
   {
      public Result<string[]> Execute(string arguments) => Git.Execute(arguments).EnumerableToResult();

      public Result<string[]> Log(string arguments) => Git.Log(arguments).EnumerableToResult();

      public Result<string[]> Fetch() => Git.Fetch().EnumerableToResult();

      public Result<string[]> ShortStatus() => Git.ShortStatus().EnumerableToResult();
   }
}