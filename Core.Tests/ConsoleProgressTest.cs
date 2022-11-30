using Core.Applications;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

namespace Core.Tests;

[TestClass]
public class ConsoleProgressTest
{
   [TestMethod]
   public void BasicTest()
   {
      var labels = array("alpha", "bravo", "charlie", "delta", "echo", "foxtrot");
      var progress = new ConsoleProgress(10, 30);
      foreach (var label in labels)
      {
         progress.Progress(label, true);
      }
   }
}