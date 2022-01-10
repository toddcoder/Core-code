using System;
using Core.Markup.Html;
using Core.Markup.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
   [TestClass]
   public class MarkupTests
   {
      [TestMethod]
      public void StyleBuildTest()
      {
         var builder = new StyleBuilder();
         builder.Add("table, th, td", "border", "1px solid black");
         builder.Add("border-collapse", "collapse");
         builder.Add("padding", "5px");
         builder.Add("font-family", "Verdana");

         builder.Add("tr:nth-child(even)", "color", "white");
         builder.Add("background-color", "salmon");

         Console.WriteLine(builder);
      }

      [TestMethod]
      public void ImplicitAttributeTest()
      {
         var builder = new MarkupBuilder("alpha");
         builder.Root.Attributes["bar"] = "@bar='txt'";
         builder.Root.Attribute = "baz='txt2'";
         Console.WriteLine(builder);
      }
   }
}
