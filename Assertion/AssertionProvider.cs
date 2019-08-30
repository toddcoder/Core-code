namespace Core.Assertion
{
   public class AssertionProvider
   {
      protected INotProvider notProvider;

      public AssertionProvider(INotProvider notProvider)
      {
         this.notProvider = notProvider;
      }
   }
}