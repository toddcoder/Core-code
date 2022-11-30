namespace Core.Strings.Text;

internal class EditLengthResult
{
   public int EditLength { get; set; }

   public int OldStart { get; set; }

   public int OldEnd { get; set; }

   public int NewStart { get; set; }

   public int NewEnd { get; set; }

   public EditType LastEdit { get; set; }
}