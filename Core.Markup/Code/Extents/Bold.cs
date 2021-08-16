namespace Core.Markup.Code.Extents
{
   public class Bold : Extent
   {
      public Bold(bool active)
      {
         Active = active;
      }

      public bool Active { get; }
   }
}