namespace Core.Git
{
   public abstract class GitResult
   {
      public static GitResult Success => new GitSuccess();

      public static GitResult Error => new GitError();

      public static implicit operator GitResult(string text) => new GitLine(text);
   }
}