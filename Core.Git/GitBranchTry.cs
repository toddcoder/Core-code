using System;
using Core.Monads;

namespace Core.Git
{
   public class GitBranchTry
   {
      protected GitBranch branch;

      public GitBranchTry(GitBranch branch)
      {
         this.branch = branch;
      }

      public Result<string[]> Delete(bool force = false) => branch.Delete(force).EnumerableToResult();

      [Obsolete("Use CheckOut instead")]
      public Result<string[]> Checkout(bool force = false) => branch.CheckOut(force).EnumerableToResult();

      public Result<string[]> CheckOut(bool force = false) => branch.CheckOut(force).EnumerableToResult();

      public Result<GitBranch> Create(string newBranchName) => branch.Create(newBranchName).EnumerableToResult().Map(_ => (GitBranch)newBranchName);

      public Result<string[]> Merge() => branch.Merge().EnumerableToResult();

      public Result<string[]> Abort() => branch.Abort().EnumerableToResult();

      public Result<string[]> Pull() => branch.Pull().EnumerableToResult();

      public Result<string[]> Push(bool first = false) => branch.Push(first).EnumerableToResult();

      public Result<string[]> Reset() => branch.Reset().EnumerableToResult();

      public Result<string[]> DifferentFromCurrent() => branch.DifferentFromCurrent().EnumerableToResult();

      public Result<string[]> DifferentFrom(GitBranch parentBranch, bool includeStatus)
      {
         return branch.DifferentFrom(parentBranch, includeStatus).EnumerableToResult();
      }
   }
}