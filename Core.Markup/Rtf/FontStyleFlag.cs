using System;

namespace Core.Markup.Rtf;

[Flags]
public enum FontStyleFlag
{
   None = 0x0,
   Bold = 0x01,
   Italic = 0x02,
   Underline = 0x04,
   Super = 0x08,
   Sub = 0x10,
   Scaps = 0x20,
   Strike = 0x40
}