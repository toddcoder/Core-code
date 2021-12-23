using Core.Monads;
using static Core.Git.GitFunctions;

namespace Core.Git
{
   public class GitTry
   {
      public Result<string[]> Execute(string arguments) => enumerableToResult(Git.Execute(arguments));

      public Result<string[]> Log(string arguments) => enumerableToResult(Git.Log(arguments));

      public Result<string[]> Fetch() => enumerableToResult(Git.Fetch());

      public Result<string[]> ShortStatus() => enumerableToResult(Git.ShortStatus());
   }
}