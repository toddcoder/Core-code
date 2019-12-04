namespace Core.Strings.Text
{
   public class EditLengthResult
   {
      public int EditLength { get; set; }

      public int StartX { get; set; }

      public int EndX { get; set; }

      public int StartY { get; set; }

      public int EndY { get; set; }

      public EditType LastEdit { get; set; }
   }
}