using System;
using System.IO;
using Core.Assertions;
using Core.Monads;

namespace Core.Computers;

public static class FullPathFunctions
{
   internal static Result<string> ValidatePath(IFullPath fullPathObject, bool allowRelativePaths = false)
   {
      try
      {
         var path = fullPathObject.FullPath;
         var fullPath = Path.GetFullPath(path);
         if (allowRelativePaths)
         {
            return Path.IsPathRooted(path).Must().BeTrue().OrFailure("Path not rooted").Map(_ => fullPath);
         }
         else
         {
            var root = Path.GetPathRoot(path);
            return root.Trim('\\', '/').Must().Not.BeNullOrEmpty().OrFailure("Root nothing but slashes and backslashes").Map(_ => fullPath);
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}