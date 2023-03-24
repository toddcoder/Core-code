using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class Hyperlink
{
   internal Hyperlink(string link, string linkTip)
   {
      Link = link;
      LinkTip = linkTip;
   }

   internal Hyperlink(string link)
   {
      Link = link;
      LinkTip = nil;
   }

   public string Link { get; }

   public Optional<string> LinkTip { get; }

   public void Deconstruct(out string link, out Optional<string> linkTip)
   {
      link = Link;
      linkTip = LinkTip;
   }
}