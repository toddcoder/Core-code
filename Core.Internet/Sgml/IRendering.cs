using System;
using Core.Computers;

namespace Core.Internet.Sgml
{
   public interface IRendering
   {
      string ToStringRendering(Func<Element, bool> callback);

      void RenderToFile(FileName file);

      void RenderToFile(FileName file, Func<Element, bool> callback);
   }
}