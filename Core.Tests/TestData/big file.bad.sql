USE local_tebennett
/************************************************************************************************
-- © Copyright 2012 - 2016, Enterprise Products Partners L.P. (Enterprise), All Rights Reserved.
-- Permission to use, copy, modify, or distribute this software source code, binaries or
-- related documentation, is strictly prohibited, without written consent from Enterprise.
-- For inquiries about the software, contact Enterprise: Enterprise Products Company Law
-- Department, 1100 Louisiana, 10th Floor, Houston, Texas 77002, phone 713-381-6500.
*************************************************************************************************/
DELETE FROM PreFlow.NominationReceiptComponent
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PreFlow.NominationReceiptComponent
      on       NominationReceiptComponent.NominationReceiptId=NominationReceipt.NominationReceiptId AND NominationReceiptComponent.ProductId=NominationReceipt.ProductId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceiptProposedTiming
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PreFlow.NominationReceiptProposedTiming on NominationReceiptProposedTiming.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingEngineRerun on BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingEngineRerunDetail on BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingEngineRerun on BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingRecordError on BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryHeaderComment on JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine on JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine on JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.DistributionRecordBillingStatusXref on DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.EstimatedDistributionRecord on EstimatedDistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionDailyQuantityComponentDetail
      on       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionPercentageComponentDetail
      on       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionTotalQuantityComponentDetail
      on       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PreFlow.NominationConsolidationDeadlineSnapshotDetail on NominationConsolidationDeadlineSnapshotDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PreFlow.NominationConsolidationDetail on NominationConsolidationDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.PaperTicketNominationAssignment
      on       PaperTicketNominationAssignment.NominationReceiptId=NominationReceipt.NominationReceiptId AND PaperTicketNominationAssignment.NominationId=NominationReceipt.NominationId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceipt
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingEngineRerun on BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingEngineRerunDetail on BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingEngineRerun on BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingRecordError on BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryHeaderComment on JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine on JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine on JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.DistributionRecordBillingStatusXref on DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.EstimatedDistributionRecord on EstimatedDistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionDailyQuantityComponentDetail
      on       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionPercentageComponentDetail
      on       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionTotalQuantityComponentDetail
      on       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PreFlow.NominationConsolidationDeadlineSnapshotDetail on NominationConsolidationDeadlineSnapshotDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PreFlow.NominationConsolidationDetail on NominationConsolidationDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PostFlow.PaperTicketNominationAssignment
      on       PaperTicketNominationAssignment.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND PaperTicketNominationAssignment.NominationId=NominationDelivery.NominationId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryComponent
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PreFlow.NominationDeliveryComponent
      on       NominationDeliveryComponent.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND NominationDeliveryComponent.ProductId=NominationDelivery.ProductId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryProposedTiming
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PreFlow.NominationDeliveryProposedTiming on NominationDeliveryProposedTiming.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationLineUpMember
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    join PreFlow.NominationLineUpMember on NominationLineUpMember.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDelivery
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationHeader
  FROM PreFlow.NominationMaster
    join PreFlow.NominationHeader on NominationHeader.NominationId=NominationMaster.NominationMasterId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingEngineRerun on BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingEngineRerunDetail on BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingEngineRerun on BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingRecordError on BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryHeaderComment on JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine on JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine on JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.DistributionRecordBillingStatusXref on DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.DistributionRecord on DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.EstimatedDistributionRecord on EstimatedDistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionDailyQuantityComponentDetail
      on       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionPercentageComponentDetail
      on       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionTotalQuantityComponentDetail
      on       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PreFlow.NominationConsolidationDeadlineSnapshotDetail on NominationConsolidationDeadlineSnapshotDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PreFlow.NominationConsolidationDetail on NominationConsolidationDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PostFlow.PaperTicketNominationAssignment
      on       PaperTicketNominationAssignment.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND PaperTicketNominationAssignment.NominationId=NominationDelivery.NominationId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryComponent
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PreFlow.NominationDeliveryComponent
      on       NominationDeliveryComponent.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND NominationDeliveryComponent.ProductId=NominationDelivery.ProductId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryProposedTiming
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PreFlow.NominationDeliveryProposedTiming on NominationDeliveryProposedTiming.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationLineUpMember
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    join PreFlow.NominationLineUpMember on NominationLineUpMember.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDelivery
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery on NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingEngineRerun
      on       BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingEngineRerunDetail
      on       BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingEngineRerun
      on       BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingRecordError
      on       BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryHeaderComment
      on       JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    join Accounting.DistributionRecordBillingStatusXref
      on       DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.EstimatedDistributionRecord
      on       EstimatedDistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionDailyQuantityComponentDetail
      on       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionPercentageComponentDetail
      on       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionTotalQuantityComponentDetail
      on       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PreFlow.NominationConsolidationDeadlineSnapshotDetail
      on       NominationConsolidationDeadlineSnapshotDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PreFlow.NominationConsolidationDetail
      on       NominationConsolidationDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PostFlow.PaperTicketNominationAssignment
      on       PaperTicketNominationAssignment.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND PaperTicketNominationAssignment.NominationId=NominationDelivery.NominationId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryComponent
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PreFlow.NominationDeliveryComponent
      on       NominationDeliveryComponent.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND NominationDeliveryComponent.ProductId=NominationDelivery.ProductId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryProposedTiming
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PreFlow.NominationDeliveryProposedTiming
      on       NominationDeliveryProposedTiming.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationLineUpMember
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    join PreFlow.NominationLineUpMember
      on       NominationLineUpMember.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDelivery
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationDelivery
      on       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationMasterDeliveryConfirmRejectReason
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationMasterDeliveryConfirmRejectReason on NominationMasterDeliveryConfirmRejectReason.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationMasterDelivery
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterDelivery on NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceiptComponent
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PreFlow.NominationReceiptComponent
      on       NominationReceiptComponent.NominationReceiptId=NominationReceipt.NominationReceiptId AND NominationReceiptComponent.ProductId=NominationReceipt.ProductId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceiptProposedTiming
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PreFlow.NominationReceiptProposedTiming on NominationReceiptProposedTiming.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingEngineRerun on BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingEngineRerunDetail on BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingEngineRerun on BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingRecordError on BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryHeaderComment on JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine on JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine on JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader on JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord on BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.DistributionRecordBillingStatusXref on DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.DistributionRecord on DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.EstimatedDistributionRecord on EstimatedDistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionDailyQuantityComponentDetail
      on       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionPercentageComponentDetail
      on       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionTotalQuantityComponentDetail
      on       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PreFlow.NominationConsolidationDeadlineSnapshotDetail on NominationConsolidationDeadlineSnapshotDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PreFlow.NominationConsolidationDetail on NominationConsolidationDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    join PostFlow.PaperTicketNominationAssignment
      on       PaperTicketNominationAssignment.NominationReceiptId=NominationReceipt.NominationReceiptId AND PaperTicketNominationAssignment.NominationId=NominationReceipt.NominationId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceipt
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt on NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceiptComponent
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PreFlow.NominationReceiptComponent
      on       NominationReceiptComponent.NominationReceiptId=NominationReceipt.NominationReceiptId AND NominationReceiptComponent.ProductId=NominationReceipt.ProductId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceiptProposedTiming
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PreFlow.NominationReceiptProposedTiming
      on       NominationReceiptProposedTiming.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingEngineRerun
      on       BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingEngineRerunDetail
      on       BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingEngineRerun
      on       BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.BillingRecordError
      on       BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryHeaderComment
      on       JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    join Accounting.ManualJournalEntryLine
      on       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    join Accounting.JournalEntryLine
      on       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    join Accounting.JournalEntryHeader
      on       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.BillingRecord
      on       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    join Accounting.DistributionRecordBillingStatusXref
      on       DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.DistributionRecord
      on       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.EstimatedDistributionRecord
      on       EstimatedDistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionDailyQuantityComponentDetail
      on       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionDailyQuantityDetail
      on       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionPercentageComponentDetail
      on       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionPercentageDetail
      on       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionTotalQuantityComponentDetail
      on       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.CustomerInstructionTotalQuantityDetail
      on       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PreFlow.NominationConsolidationDeadlineSnapshotDetail
      on       NominationConsolidationDeadlineSnapshotDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PreFlow.NominationConsolidationDetail
      on       NominationConsolidationDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    join PostFlow.PaperTicketNominationAssignment
      on       PaperTicketNominationAssignment.NominationReceiptId=NominationReceipt.NominationReceiptId AND PaperTicketNominationAssignment.NominationId=NominationReceipt.NominationId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceipt
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationReceipt
      on       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationMasterReceiptConfirmRejectReason
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    join PreFlow.NominationMasterReceiptConfirmRejectReason on NominationMasterReceiptConfirmRejectReason.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationMasterReceipt
  FROM PreFlow.NominationMaster
    join PreFlow.NominationMasterReceipt on NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationMaster
 where NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);