using System;
using Core.Markup.Html;
using Core.Markup.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

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
      var alpha = builder.Root;
      alpha += "@bar=txt";
      alpha += "baz=txt2";
      Console.WriteLine(alpha);
   }

   [TestMethod]
   public void ImplicitElementTest()
   {
      var builder = new MarkupBuilder("alpha");
      var alpha = builder.Root;
      alpha *= "bar>txt";
      alpha *= "baz>' txt2 ";
      Console.WriteLine(alpha);
   }

   [TestMethod]
   public void PlusElementTest()
   {
      var builder = new MarkupBuilder("alpha");
      var alpha = builder.Root;
      var bar = alpha + "bar>";
      _ = bar + "baz>none";

      Console.WriteLine(builder);
   }

   [TestMethod]
   public void HtmlTest()
   {
      var builder = new HtmlBuilder();
      var tableThTd = builder + "table, th, td { border: 1px solid black";
      tableThTd += "border-collapse: collapse";
      tableThTd += "padding: 5px";
      _ = tableThTd + "font-family: Verdana";

      var nthChild = builder + "tr:nth-child(even) {";
      nthChild += "color: white";
      _ = nthChild + "background-color: salmon";

      var table = builder.Body + "table>";
      var tr = table + "tr>";
      tr *= "th>Alpha";
      tr *= "th>Bravo";
      _ = tr * "th>Charlie";

      tr = table + "tr>";
      tr *= "td>alpha";
      tr *= "td>beta";
      _ = tr * "td>kappa";

      tr = table + "tr>";
      tr *= "td>ah";
      tr *= "td>bo";
      _ = tr * "td>tso";

      Console.WriteLine(builder);
   }
}