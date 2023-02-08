using System;
using Core.Markup.Html;
using Core.Markup.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

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

   [TestMethod]
   public void HtmlTest2()
   {
      var builder = new HtmlBuilder();

      var defaultStyle = builder + ".header{";
      _ = defaultStyle + "color: white";
      _ = defaultStyle + "background-color: blue";

      var titleStyle = builder + ".title{";
      _ = titleStyle + "font-weight: bold";
      _ = titleStyle + "font-size: 16px";

      var boldStyle = builder + ".bold{";
      _ = boldStyle + "font-weight: bold";
      _ = boldStyle + "font-size: 14px";

      var body = builder.Body;
      _ = body + "@style='font-family: Verdana; font-size: 11px'";

      var p = body + "p>Merged Branches";
      _ = p + "@class=title";

      var table = body + "table>";
      _ = table + "@border=1px black solid";
      var th = table + "th>";
      _ = th + "@class=header";
      _ = th + "b>Branch";

      var tr = table + "tr>";
      _ = tr + "td>Alpha";
      tr = table + "tr>";
      _ = tr + "td>Bravo";
      tr = table + "tr>";
      _ = tr + "td>Charlie";

      _ = body + "hr>";
      p = body + "p>Conflicted Branches";
      _ = p + "@class=title";

      foreach (var branch in array("branch1", "branch2", "branch3"))
      {
         _ = body + "hr>";

         table = body + "table>";
         _ = table + "@border=1px black solid";
         th = table + $"th>{branch}";
         _ = th + "@class=header";
         th = table + "th>File";
         _ = th + "@class=header";

         foreach (var file in array("file1", "file2", "file3", "file4"))
         {
            tr = table + "tr>";
            _ = tr + "td>";
            _ = tr + $"td>{file}";
         }
      }

      Console.WriteLine(builder);
   }
}