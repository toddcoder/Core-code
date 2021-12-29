using Core.Monads;
using static Core.Git.GitFunctions;

namespace Core.Git
{
   public class GitBranchTry
   {
      protected GitBranch branch;

      public GitBranchTry(GitBranch branch)
      {
         this.branch = branch;
      }

      public Result<string[]> Delete(bool force = false) => enumerableToResult(branch.Delete(force));

      public Result<string[]> Checkout(bool force = false) => enumerableToResult(branch.CheckOut(force));

      public Result<GitBranch> Create(string newBranchName) => enumerableToResult(branch.Create(newBranchName)).Map(_ => (GitBranch)newBranchName);

      public Result<string[]> Merge() => enumerableToResult(branch.Merge());

      public Result<string[]> Abort() => enumerableToResult(branch.Abort());

      public Result<string[]> Pull() => enumerableToResult(branch.Pull());

      public Result<string[]> Push(bool first = false) => enumerableToResult(branch.Push(first));

      public Result<string[]> Reset() => enumerableToResult(branch.Reset());

      public Result<string[]> DifferentFromCurrent() => enumerableToResult(branch.DifferentFromCurrent());

      public Result<string[]> DifferentFrom(GitBranch parentBranch, bool includeStatus)
      {
         return enumerableToResult(branch.DifferentFrom(parentBranch, includeStatus));
      }
   }
}