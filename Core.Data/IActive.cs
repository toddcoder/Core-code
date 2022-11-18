namespace Core.Data;

public interface IActive
{
   void BeforeExecute();
   void AfterExecute();
}