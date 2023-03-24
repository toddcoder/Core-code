using Core.Monads;

namespace Core.Git;

public class GitTry
{
   public Optional<string[]> Execute(string arguments) => Git.Execute(arguments).EnumerableToResult();

   public Optional<string[]> Log(string arguments) => Git.Log(arguments).EnumerableToResult();

   public Optional<string[]> Fetch() => Git.Fetch().EnumerableToResult();

   public Optional<string[]> ShortStatus() => Git.ShortStatus().EnumerableToResult();

   public Optional<string[]> CherryPick(string reference) => Git.CherryPick(reference).EnumerableToResult();
}