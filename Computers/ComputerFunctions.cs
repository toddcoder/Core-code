using System;
using Core.Strings;

namespace Core.Computers;

public static class ComputerFunctions
{
   public static string replaceTilde(string path)
   {
      if (path.StartsWith("~"))
      {
         return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + path.Drop(1);
      }
      else
      {
         return path;
      }
   }
}