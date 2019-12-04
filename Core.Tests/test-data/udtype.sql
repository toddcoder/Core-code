---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
/************************************************************************************************
*
*  © Copyright 2012, Enterprise Products Partners L.P. (Enterprise), All Rights Reserved.
*  Permission to use, copy, modify, or distribute this software source code, binaries or 
*  related documentation, is strictly prohibited, without written consent from Enterprise. 
*  For inquiries about the software, contact Enterprise: Enterprise Products Company Law
*  Department, 1100 Louisiana, 10th Floor, Houston, Texas 77002, phone 713-381-6500.
*
************************************************************************************************/
CREATE OR ALTER PROCEDURE [Measurement].[uspSaveMeasurementValues]
    @pPollRequestId BIGINT
   ,@pPollResponseId BIGINT
   ,@pMeasurementValues [Measurement].[uttMeasurementValues] READONLY
AS
BEGIN
    SET NOCOUNT ON;
    /* 
-- Brief functionality of stored procedure
The stored procedure is used to save the measurement values for a poll response.
-- Called by
This stored procedure will be called by the Polling Integration Service
  
-- Input Parameters :
-- @pPollRequestId = the poll request id to associate the responses to.
-- @@PollResponseId	=	the poll response id 
-- @pPollResponseValues = the list of response values and assosiated register addresses.
-- Output Parameters: None 
*/
    DECLARE @ContextMessage VARCHAR(4000);

    SET @ContextMessage
        = 'Begin inserting measurement Values for PollRequestId = ' + CONVERT(VARCHAR(100), @pPollRequestId)
          + ' and PollResponseId = ' + CONVERT(VARCHAR, @pPollResponseId);

    BEGIN TRY
        BEGIN TRANSACTION;
        INSERT INTO Measurement.MeasurementValue
        (
            PollResponseValueId
           ,MeasurementAttributeId
           ,MeasurementValue
        )
        SELECT PollResponseValue.PollResponseValueId
              ,response.MeasurementAttributeId
              ,response.Value
          FROM @pMeasurementValues                     AS response
              INNER JOIN Measurement.PollRequest       PollRequest
                      ON PollRequest.PollRequestId = @pPollRequestId
              INNER JOIN Measurement.DevicePollRequest DevicePollRequest
                      ON PollRequest.DeviceRequestTemplateId = DevicePollRequest.DeviceRequestTemplateId
                         AND DevicePollRequest.Address = response.StartingAddress
              INNER JOIN Measurement.PollResponseValue
                      ON PollResponseValue.DevicePollRequestId = DevicePollRequest.DevicePollRequestId
                         AND PollResponseValue.PollResponseId = @pPollResponseId
         WHERE ISNULL(response.MeasurementAttributeId, 0) > 0;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        SET NOCOUNT OFF;
        EXEC dbo.uspRethrowError @ContextMessage;
    END CATCH;

END;
--end of Measurement.uspSaveMeasurementValues 
