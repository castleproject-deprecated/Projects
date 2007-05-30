IF EXISTS (SELECT ID FROM sysobjects WHERE NAME = 'spSCHED_RaiseError' AND type = 'p')
    DROP PROCEDURE spSCHED_RaiseError
GO

/******************************************************************************
** Name    : spSCHED_RaiseError
**
** Summary:
**
**   Raises an error.
**   When used within a CATCH block, includes information about the cause of
**   original error in the message.
**
** Example:

BEGIN TRY
	SELECT 1/0
END TRY
BEGIN CATCH
	EXEC spSCHED_RaiseError 'spMyBrokenStoredProcedure', 'Something bad happened when dividing 1 by 0'
END CATCH

** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   05/20/07 Jeff              Initial implementation.
*******************************************************************************
** Copyright (C) 2007 Castle Project, All Rights Reserved
*******************************************************************************/

CREATE PROCEDURE dbo.spSCHED_RaiseError
(
	@ModuleName NVARCHAR(200),
	@ErrorDetails NVARCHAR(MAX)
)
AS BEGIN
	SET NOCOUNT ON
	SET LOCK_TIMEOUT 30000
	
	IF ERROR_NUMBER() IS NOT NULL
	BEGIN
		DECLARE 
			@ErrorMessage    NVARCHAR(4000),
			@ErrorNumber     INT,
			@ErrorSeverity   INT,
			@ErrorState      INT,
			@ErrorLine       INT,
			@ErrorProcedure  NVARCHAR(200);

		SELECT 
			@ErrorNumber = ERROR_NUMBER(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE(),
			@ErrorLine = ERROR_LINE(),
			@ErrorProcedure = ISNULL(ERROR_PROCEDURE(), '<unknown>'),
			@ErrorMessage = ERROR_MESSAGE();
		
		-- Compose an error message with all of the details we have.
		RAISERROR('%s - %s --> Error %d, Procedure %s, Line %d: %s', @ErrorSeverity, @ErrorState, @ModuleName, @ErrorDetails, @ErrorNumber, @ErrorProcedure, @ErrorLine, @ErrorMessage)
	END
	ELSE
	BEGIN
		-- No extra error information!
		-- Raise the error anyways using what we know.
		RAISERROR('%s - %s', 16, 1, @ModuleName, @ErrorDetails)
	END
END
GO

GRANT EXECUTE ON dbo.spSCHED_RaiseError TO SchedulerRole
GO
