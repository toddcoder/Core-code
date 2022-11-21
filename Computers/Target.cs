namespace Core.Computers;

public class Target
{
   public enum Option
   {
      Overwrite,
      Unique
   }

   public static FileName operator |(Target target, FileName file)
   {
      file.CopyTo(target.folder, target.Overwrite);
      return file;
   }

   public static FileName operator &(Target target, FileName file) => file.MoveTo(target.folder, target.Overwrite, target.Unique);

   protected FolderName folder;
   protected Option option;

   internal Target(FolderName folder, Option option)
   {
      this.folder = folder;
      this.option = option;
   }

   public bool Overwrite => option == Option.Overwrite;

   public bool Unique => option == Option.Unique;
}