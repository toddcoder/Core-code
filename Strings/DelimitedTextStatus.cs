using System;

namespace Core.Strings;

[Flags]
public enum DelimitedTextStatus
{
   Outside = 1,
   Inside = 2,
   BeginDelimiter = 4,
   EndDelimiter = 8
}