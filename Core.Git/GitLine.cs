namespace Core.Git
{
   public sealed class GitLine : GitResult
   {
      public GitLine(string text)
      {
         Text = text;
      }

      public string Text { get; }

      public override string ToString() => Text;
   }
}