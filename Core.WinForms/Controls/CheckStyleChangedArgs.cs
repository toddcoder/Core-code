using System;

namespace Core.WinForms.Controls;

public class CheckStyleChangedArgs
{
   public CheckStyleChangedArgs(Guid id, CheckStyle checkStyle)
   {
      Id = id;
      CheckStyle = checkStyle;
   }

   public Guid Id { get; }

   public CheckStyle CheckStyle { get; }
}