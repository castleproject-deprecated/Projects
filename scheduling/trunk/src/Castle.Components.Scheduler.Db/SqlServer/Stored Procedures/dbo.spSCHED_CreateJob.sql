-- Copyright 2007 Castle Project - http://www.castleproject.org/
-- 
-- Licensed under the Apache License, Version 2.0 (the "License");
-- you may not use this file except in compliance with the License.
-- You may obtain a copy of the License at
-- 
--     http://www.apache.org/licenses/LICENSE-2.0
-- 
-- Unless required by applicable law or agreed to in writing, software
-- distributed under the License is distributed on an "AS IS" BASIS,
-- WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
-- See the License for the specific language governing permissions and
-- limitations under the License.

IF EXISTS (SELECT ID FROM sysobjects WHERE NAME = 'spSCHED_CreateJob' AND type = 'p')
    DROP PROCEDURE spSCHED_CreateJob
GO

/******************************************************************************
** Name    : spSCHED_CreateJob
**
** Summary:
**
**   Creates a job in the database.
**
** Example:

DECLARE @CreationTime DATETIME
SET @CreationTime = GETUTCDATE()
DECLARE @WasCreated BIT
EXEC spSCHED_CreateJob 'TestCluster', 'job', 'Test job.', 'job.key', NULL, NULL, @CreationTime, 0, @WasCreated OUTPUT
SELECT @WasCreated

** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   05/20/07 Jeff              Initial implementation.
*******************************************************************************
** Copyright (C) 2007 Castle Project, All Rights Reserved
*******************************************************************************/

CREATE PROCEDURE dbo.spSCHED_CreateJob
(
	@ClusterName NVARCHAR(200),
	@JobName NVARCHAR(200),
	@JobDescription NVARCHAR(1000),
	@JobKey NVARCHAR(200),
	@TriggerObject VARBINARY(MAX),
	@JobDataObject VARBINARY(MAX),
	@CreationTime DATETIME,
	@ReplaceExisting BIT,
	@WasCreated BIT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON
	SET LOCK_TIMEOUT 30000
	
	DECLARE @JobID INT
	
	DECLARE @JobState_Pending INT
	SET @JobState_Pending = 0
	
	SET @WasCreated = 0
	
	BEGIN TRY
		BEGIN TRAN
	
		-- Find the job if it already exists.
		SELECT @JobID = J.JobID
			FROM SCHED_Jobs J
			INNER JOIN SCHED_Clusters C ON C.ClusterID = J.ClusterID
			WHERE C.ClusterName = @ClusterName AND J.JobName = @JobName
		
		-- Handle the replacement / ignore cases.	
		IF @JobID IS NOT NULL
		BEGIN
			IF @ReplaceExisting <> 0
			BEGIN
				DELETE FROM SCHED_Jobs
					WHERE JobID = @JobID
			END ELSE BEGIN
				IF XACT_STATE() <> 0 ROLLBACK
				RETURN
			END
		END
		
		-- Find the cluster.
		DECLARE @ClusterID INT
		SELECT @ClusterID = ClusterID
			FROM SCHED_Clusters
			WHERE ClusterName = @ClusterName
			
		IF @ClusterID IS NULL
		BEGIN
			IF XACT_STATE() <> 0 ROLLBACK
			EXEC spSCHED_RaiseError 'spSCHED_CreateJob', 'Could not create job because cluster name was not registered.'
			RETURN
		END
		
		-- Insert the new record
		INSERT INTO SCHED_Jobs (ClusterID, JobName, JobDescription, JobKey, TriggerObject, JobDataObject, CreationTime, JobState)
			VALUES (@ClusterID, @JobName, @JobDescription, @JobKey, @TriggerObject, @JobDataObject, @CreationTime, @JobState_Pending)
		SET @JobID = SCOPE_IDENTITY()
		SET @WasCreated = 1
	
		COMMIT
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0 ROLLBACK
		EXEC spSCHED_RaiseError 'spSCHED_CreateJob', 'Could not create job.'
	END CATCH
END
GO

GRANT EXECUTE ON dbo.spSCHED_CreateJob TO SchedulerRole
GO
