namespace Core.Computers;

public class ImpersonatorArguments
{
   public ImpersonatorArguments()
   {
      Domain = "";
      UserName = "";
      Password = "";
   }

   public string Domain { get; set; }

   public string UserName { get; set; }

   public string Password { get; set; }

   public bool Service { get; set; }
}