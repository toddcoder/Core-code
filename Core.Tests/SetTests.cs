using System;
using System.Linq;
using Core.Assertions;
using Core.Collections;
using Core.Numbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Assertions.AssertionFunctions;

namespace Core.Tests
{
   [TestClass]
   public class SetTests
   {
      [TestMethod]
      public void SetOfStringVsStringSetTest()
      {
         var setOfString = new Set<string> { "Case" };
         var stringSet = new StringSet(true) { "Case" };

         setOfString.Remove("case");
         assert(() => setOfString.Count).Must().Equal(1).OrThrow();

         stringSet.Remove("case");
         assert(() => stringSet.Count).Must().Equal(0).OrThrow();
      }

      [TestMethod]
      public void SetOperatorsTest()
      {
         var union1 = Set<int>.Of(0.Until(5).Select(i => i * 2));
         var union2 = Set<int>.Of(0.Until(5).Select(i => i * 2 + 1));
         var union = union1 | union2;
         Console.WriteLine($"({enumerableImage(union1)}) union ({enumerableImage(union2)}) = ({enumerableImage(union)})");

         var intersection1 = Set<int>.Of(3, 4);
         var intersection2 = Set<int>.Of(1, 3, 5, 2, 4);
         var intersection = intersection1 & intersection2;
         Console.WriteLine($"({enumerableImage(intersection1)}) intersection ({enumerableImage(intersection2)}) = ({enumerableImage(intersection)})");

         var exception1 = Set<int>.Of(3.Until(10));
         var exception2 = Set<int>.Of(0.Until(6));
         var exception = exception1 - exception2;
         Console.WriteLine($"({enumerableImage(exception1)}) exception ({enumerableImage(exception2)}) = ({enumerableImage(exception)})");


         var symmetricException = intersection1 ^ intersection2;
         Console.WriteLine($"({enumerableImage(intersection1)}) symmetric exception ({enumerableImage(intersection2)}) = ({enumerableImage(symmetricException)})");
      }

      [TestMethod]
      public void SetOperators2Test()
      {
         var indexes = new StringSet(true, "BillingRecordTypeId", "BillingCompanyId", "BillingRecordStatusId", "CommercialAssetId",
            "DistributionRecordId", "TariffFeeHeaderId", "ContractId", "ContractRateSheetFeeHeaderId", "BillingScheduleLineItemId", "InvoiceDetailId",
            "FlowDate", "IsEstimated");
         var found = new StringSet(true, "InvoiceDetailId", "IsEstimated", "BillingAmount");

         var result = indexes ^ found;
         Console.WriteLine($"^{enumerableImage(result.OrderBy(i => i))}");

         result = indexes - found;
         Console.WriteLine($"-{enumerableImage(result.OrderBy(i => i))}");

         result = indexes & found;
         Console.WriteLine($"&{enumerableImage(result.OrderBy(i => i))}");
      }
   }
}
