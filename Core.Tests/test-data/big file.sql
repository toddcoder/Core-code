/************************************************************************************************
-- © Copyright 2012 - 2016, Enterprise Products Partners L.P. (Enterprise), All Rights Reserved.
-- Permission to use, copy, modify, or distribute this software source code, binaries or
-- related documentation, is strictly prohibited, without written consent from Enterprise.
-- For inquiries about the software, contact Enterprise: Enterprise Products Company Law
-- Department, 1100 Louisiana, 10th Floor, Houston, Texas 77002, phone 713-381-6500.
*************************************************************************************************/
DELETE FROM PreFlow.NominationReceiptComponent
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PreFlow.NominationReceiptComponent
      ON       NominationReceiptComponent.NominationReceiptId=NominationReceipt.NominationReceiptId AND NominationReceiptComponent.ProductId=NominationReceipt.ProductId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceiptProposedTiming
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PreFlow.NominationReceiptProposedTiming ON NominationReceiptProposedTiming.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingEngineRerun ON BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingEngineRerunDetail ON BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingEngineRerun ON BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingRecordError ON BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryHeaderComment ON JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine ON JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine ON JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.DistributionRecordBillingStatusXref ON DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.EstimatedDistributionRecord ON EstimatedDistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityComponentDetail
      ON       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageComponentDetail
      ON       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityComponentDetail
      ON       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PreFlow.NominationConsolidationDeadlineSnapshotDetail ON NominationConsolidationDeadlineSnapshotDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PreFlow.NominationConsolidationDetail ON NominationConsolidationDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.PaperTicketNominationAssignment
      ON       PaperTicketNominationAssignment.NominationReceiptId=NominationReceipt.NominationReceiptId AND PaperTicketNominationAssignment.NominationId=NominationReceipt.NominationId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceipt
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationId=NominationHeader.NominationId AND NominationReceipt.VersionNumber=NominationHeader.VersionNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingEngineRerun ON BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingEngineRerunDetail ON BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingEngineRerun ON BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingRecordError ON BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryHeaderComment ON JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine ON JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine ON JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.DistributionRecordBillingStatusXref ON DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.EstimatedDistributionRecord ON EstimatedDistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityComponentDetail
      ON       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageComponentDetail
      ON       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityComponentDetail
      ON       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PreFlow.NominationConsolidationDeadlineSnapshotDetail ON NominationConsolidationDeadlineSnapshotDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PreFlow.NominationConsolidationDetail ON NominationConsolidationDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PostFlow.PaperTicketNominationAssignment
      ON       PaperTicketNominationAssignment.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND PaperTicketNominationAssignment.NominationId=NominationDelivery.NominationId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryComponent
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PreFlow.NominationDeliveryComponent
      ON       NominationDeliveryComponent.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND NominationDeliveryComponent.ProductId=NominationDelivery.ProductId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryProposedTiming
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PreFlow.NominationDeliveryProposedTiming ON NominationDeliveryProposedTiming.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationLineUpMember
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
    INNER JOIN PreFlow.NominationLineUpMember ON NominationLineUpMember.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDelivery
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationId=NominationHeader.NominationId AND NominationDelivery.VersionNumber=NominationHeader.VersionNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationHeader
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationHeader ON NominationHeader.NominationId=NominationMaster.NominationMasterId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingEngineRerun ON BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingEngineRerunDetail ON BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingEngineRerun ON BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingRecordError ON BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryHeaderComment ON JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine ON JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine ON JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.DistributionRecordBillingStatusXref ON DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.EstimatedDistributionRecord ON EstimatedDistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityComponentDetail
      ON       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageComponentDetail
      ON       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityComponentDetail
      ON       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PreFlow.NominationConsolidationDeadlineSnapshotDetail ON NominationConsolidationDeadlineSnapshotDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PreFlow.NominationConsolidationDetail ON NominationConsolidationDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PostFlow.PaperTicketNominationAssignment
      ON       PaperTicketNominationAssignment.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND PaperTicketNominationAssignment.NominationId=NominationDelivery.NominationId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryComponent
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PreFlow.NominationDeliveryComponent
      ON       NominationDeliveryComponent.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND NominationDeliveryComponent.ProductId=NominationDelivery.ProductId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryProposedTiming
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PreFlow.NominationDeliveryProposedTiming ON NominationDeliveryProposedTiming.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationLineUpMember
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
    INNER JOIN PreFlow.NominationLineUpMember ON NominationLineUpMember.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDelivery
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery ON NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingEngineRerun
      ON       BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingEngineRerunDetail
      ON       BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingEngineRerun
      ON       BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingRecordError
      ON       BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryHeaderComment
      ON       JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
    INNER JOIN Accounting.DistributionRecordBillingStatusXref
      ON       DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.EstimatedDistributionRecord
      ON       EstimatedDistributionRecord.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityComponentDetail
      ON       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageComponentDetail
      ON       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionPercentageDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityComponentDetail
      ON       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationDelivery.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationDelivery.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.DeliveryLineNumber=NominationDelivery.DeliveryLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PreFlow.NominationConsolidationDeadlineSnapshotDetail
      ON       NominationConsolidationDeadlineSnapshotDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PreFlow.NominationConsolidationDetail
      ON       NominationConsolidationDetail.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PostFlow.PaperTicketNominationAssignment
      ON       PaperTicketNominationAssignment.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND PaperTicketNominationAssignment.NominationId=NominationDelivery.NominationId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryComponent
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PreFlow.NominationDeliveryComponent
      ON       NominationDeliveryComponent.NominationDeliveryId=NominationDelivery.NominationDeliveryId AND NominationDeliveryComponent.ProductId=NominationDelivery.ProductId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDeliveryProposedTiming
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PreFlow.NominationDeliveryProposedTiming
      ON       NominationDeliveryProposedTiming.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationLineUpMember
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
    INNER JOIN PreFlow.NominationLineUpMember
      ON       NominationLineUpMember.NominationDeliveryId=NominationDelivery.NominationDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationDelivery
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationDelivery
      ON       NominationDelivery.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
     AND       NominationDelivery.NominationId=NominationMasterDelivery.NominationMasterId
     AND       NominationDelivery.DeliveryLineNumber=NominationMasterDelivery.DeliveryLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationMasterDeliveryConfirmRejectReason
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationMasterDeliveryConfirmRejectReason ON NominationMasterDeliveryConfirmRejectReason.NominationMasterDeliveryId=NominationMasterDelivery.NominationMasterDeliveryId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationMasterDelivery
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterDelivery ON NominationMasterDelivery.NominationMasterId=NominationMaster.NominationMasterId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceiptComponent
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PreFlow.NominationReceiptComponent
      ON       NominationReceiptComponent.NominationReceiptId=NominationReceipt.NominationReceiptId AND NominationReceiptComponent.ProductId=NominationReceipt.ProductId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceiptProposedTiming
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PreFlow.NominationReceiptProposedTiming ON NominationReceiptProposedTiming.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingEngineRerun ON BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingEngineRerunDetail ON BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingEngineRerun ON BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingRecordError ON BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryHeaderComment ON JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine ON JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine ON JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader ON JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord ON BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.DistributionRecordBillingStatusXref ON DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.DistributionRecord ON DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.EstimatedDistributionRecord ON EstimatedDistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityComponentDetail
      ON       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageComponentDetail
      ON       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityComponentDetail
      ON       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PreFlow.NominationConsolidationDeadlineSnapshotDetail ON NominationConsolidationDeadlineSnapshotDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PreFlow.NominationConsolidationDetail ON NominationConsolidationDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
    INNER JOIN PostFlow.PaperTicketNominationAssignment
      ON       PaperTicketNominationAssignment.NominationReceiptId=NominationReceipt.NominationReceiptId AND PaperTicketNominationAssignment.NominationId=NominationReceipt.NominationId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceipt
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt ON NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceiptComponent
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PreFlow.NominationReceiptComponent
      ON       NominationReceiptComponent.NominationReceiptId=NominationReceipt.NominationReceiptId AND NominationReceiptComponent.ProductId=NominationReceipt.ProductId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceiptProposedTiming
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PreFlow.NominationReceiptProposedTiming
      ON       NominationReceiptProposedTiming.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerunDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingEngineRerun
      ON       BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingEngineRerunDetail
      ON       BillingEngineRerunDetail.BillingEngineRerunId=BillingEngineRerun.BillingEngineRerunId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingEngineRerun
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingEngineRerun
      ON       BillingEngineRerun.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecordError
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.BillingRecordError
      ON       BillingRecordError.BillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeaderComment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryHeaderComment
      ON       JournalEntryHeaderComment.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.ManualJournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
    INNER JOIN Accounting.ManualJournalEntryLine
      ON       ManualJournalEntryLine.JournalEntryLineId=JournalEntryLine.JournalEntryLineId AND ManualJournalEntryLine.ManualJournalEntryHeaderId=JournalEntryLine.ManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryLine
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
    INNER JOIN Accounting.JournalEntryLine
      ON       JournalEntryLine.JournalEntryHeaderId=JournalEntryHeader.JournalEntryHeaderId AND JournalEntryLine.ManualJournalEntryHeaderId=JournalEntryHeader.SourceManualJournalEntryHeaderId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.JournalEntryHeader
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
    INNER JOIN Accounting.JournalEntryHeader
      ON       JournalEntryHeader.SourceBillingRecordId=BillingRecord.BillingRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.BillingRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.BillingRecord
      ON       BillingRecord.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM Accounting.DistributionRecordBillingStatusXref
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
    INNER JOIN Accounting.DistributionRecordBillingStatusXref
      ON       DistributionRecordBillingStatusXref.DistributionRecordId=DistributionRecord.DistributionRecordId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.DistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.DistributionRecord
      ON       DistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.EstimatedDistributionRecord
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.EstimatedDistributionRecord
      ON       EstimatedDistributionRecord.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityComponentDetail
      ON       CustomerInstructionDailyQuantityComponentDetail.CustomerInstructionDailyQuantityDetailId=CustomerInstructionDailyQuantityDetail.CustomerInstructionDailyQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionDailyQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionDailyQuantityDetail
      ON       CustomerInstructionDailyQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionDailyQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionDailyQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageComponentDetail
      ON       CustomerInstructionPercentageComponentDetail.CustomerInstructionPercentageDetailId=CustomerInstructionPercentageDetail.CustomerInstructionPercentageDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionPercentageDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionPercentageDetail
      ON       CustomerInstructionPercentageDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionPercentageDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionPercentageDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityComponentDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityComponentDetail
      ON       CustomerInstructionTotalQuantityComponentDetail.CustomerInstructionTotalQuantityDetailId=CustomerInstructionTotalQuantityDetail.CustomerInstructionTotalQuantityDetailId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.CustomerInstructionTotalQuantityDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.CustomerInstructionTotalQuantityDetail
      ON       CustomerInstructionTotalQuantityDetail.NominationId=NominationReceipt.NominationId
     AND       CustomerInstructionTotalQuantityDetail.NominationOriginalVersionId=NominationReceipt.VersionNumber
     AND       CustomerInstructionTotalQuantityDetail.ReceiptLineNumber=NominationReceipt.ReceiptLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDeadlineSnapshotDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PreFlow.NominationConsolidationDeadlineSnapshotDetail
      ON       NominationConsolidationDeadlineSnapshotDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationConsolidationDetail
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PreFlow.NominationConsolidationDetail
      ON       NominationConsolidationDetail.NominationReceiptId=NominationReceipt.NominationReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PostFlow.PaperTicketNominationAssignment
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
    INNER JOIN PostFlow.PaperTicketNominationAssignment
      ON       PaperTicketNominationAssignment.NominationReceiptId=NominationReceipt.NominationReceiptId AND PaperTicketNominationAssignment.NominationId=NominationReceipt.NominationId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationReceipt
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationReceipt
      ON       NominationReceipt.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
     AND       NominationReceipt.NominationId=NominationMasterReceipt.NominationMasterId
     AND       NominationReceipt.ReceiptLineNumber=NominationMasterReceipt.ReceiptLineNumber
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationMasterReceiptConfirmRejectReason
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
    INNER JOIN PreFlow.NominationMasterReceiptConfirmRejectReason ON NominationMasterReceiptConfirmRejectReason.NominationMasterReceiptId=NominationMasterReceipt.NominationMasterReceiptId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);

DELETE FROM PreFlow.NominationMasterReceipt
  FROM PreFlow.NominationMaster
    INNER JOIN PreFlow.NominationMasterReceipt ON NominationMasterReceipt.NominationMasterId=NominationMaster.NominationMasterId
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);


DELETE FROM PreFlow.NominationMaster
 WHERE NominationMaster.NominationMasterId IN (24, 25, 26, 27, 28);