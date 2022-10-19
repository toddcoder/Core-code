using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class LocalHyperlink
{
   public LocalHyperlink(string link, string linkTip)
   {
      Link = link;
      LinkTip = linkTip;
   }

   public LocalHyperlink(string link)
   {
      Link = link;
      LinkTip = nil;
   }

   public string Link { get; }

   public Maybe<string> LinkTip { get; }

   public void Deconstruct(out string link, out Maybe<string> linkTip)
   {
      link = Link;
      linkTip = LinkTip;
   }
}