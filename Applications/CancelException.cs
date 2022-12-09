using System;

namespace Core.Applications;

public class CancelException : ApplicationException
{
   public CancelException() : base("Cancelled")
   {
   }
}