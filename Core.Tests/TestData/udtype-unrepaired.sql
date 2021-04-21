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
create or alter procedure [Measurement].[uspSaveMeasurementValues]
    @pPollRequestId bigint
   ,@pPollResponseId bigint
   ,@pMeasurementValues [Measurement].[uttMeasurementValues] READONLY
as
begin
    set nocount on;
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
    declare @ContextMessage varchar(4000);

    set @ContextMessage
        = 'Begin inserting measurement Values for PollRequestId = ' + convert(varchar(100), @pPollRequestId)
          + ' and PollResponseId = ' + convert(varchar, @pPollResponseId);

    begin try
        begin transaction;
        insert into Measurement.MeasurementValue
        (
            PollResponseValueId
           ,MeasurementAttributeId
           ,MeasurementValue
        )
        select PollResponseValue.PollResponseValueId
              ,response.MeasurementAttributeId
              ,response.Value
          from @pMeasurementValues                     as response
              join Measurement.PollRequest       PollRequest
                      on PollRequest.PollRequestId = @pPollRequestId
              join Measurement.DevicePollRequest DevicePollRequest
                      on PollRequest.DeviceRequestTemplateId = DevicePollRequest.DeviceRequestTemplateId
                         and DevicePollRequest.Address = response.StartingAddress
              join Measurement.PollResponseValue
                      on PollResponseValue.DevicePollRequestId = DevicePollRequest.DevicePollRequestId
                         and PollResponseValue.PollResponseId = @pPollResponseId
         where ISNULL(response.MeasurementAttributeId, 0) > 0;
        commit transaction;
    end try
    begin catch
        rollback transaction;

        set nocount off;
        exec dbo.uspRethrowError @ContextMessage;
    end catch;

end;
--end of Measurement.uspSaveMeasurementValues 
