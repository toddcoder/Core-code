using Core.Monads;

namespace Core.Git;

public class GitBranchTry
{
   protected GitBranch branch;

   public GitBranchTry(GitBranch branch)
   {
      this.branch = branch;
   }

   public Optional<string[]> Delete(bool force = false) => branch.Delete(force).EnumerableToResult();

   public Optional<string[]> CheckOut(bool force = false) => branch.CheckOut(force).EnumerableToResult();

   public Optional<GitBranch> Create(string newBranchName) => branch.Create(newBranchName).EnumerableToResult().Map(_ => (GitBranch)newBranchName);

   public Optional<string[]> Merge() => branch.Merge().EnumerableToResult();

   public Optional<string[]> Abort() => branch.Abort().EnumerableToResult();

   public Optional<string[]> Pull() => branch.Pull().EnumerableToResult();

   public Optional<string[]> Push(bool first = false) => branch.Push(first).EnumerableToResult();

   public Optional<string[]> Reset() => branch.Reset().EnumerableToResult();

   public Optional<string[]> DifferentFromCurrent() => branch.DifferentFromCurrent().EnumerableToResult();

   public Optional<string[]> DifferentFrom(GitBranch parentBranch, bool includeStatus)
   {
      return branch.DifferentFrom(parentBranch, includeStatus).EnumerableToResult();
   }
}