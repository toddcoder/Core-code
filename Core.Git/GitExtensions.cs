using System.Collections.Generic;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Git;

internal static class GitExtensions
{
   private enum GitStatus
   {
      NotSet,
      Success,
      Error
   }

   public static Optional<string[]> EnumerableToResult(this IEnumerable<GitResult> enumerable)
   {
      var list = new List<string>();
      var status = GitStatus.NotSet;

      foreach (var result in enumerable)
      {
         switch (result)
         {
            case GitSuccess:
               status = GitStatus.Success;
               break;
            case GitError:
               status = GitStatus.Error;
               break;
            case GitLine gitLine:
               list.Add(gitLine.Text);
               break;
         }
      }

      return status switch
      {
         GitStatus.NotSet => fail("No lines returned"),
         GitStatus.Success => list.ToArray(),
         GitStatus.Error => fail(list.ToString(" ")),
         _ => fail("Argument out of range")
      };
   }
}