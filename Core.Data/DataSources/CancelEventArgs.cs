using System;

namespace Core.Data.DataSources;

public class CancelEventArgs : EventArgs
{
   public CancelEventArgs() => Cancel = false;

   public bool Cancel { get; set; }
}