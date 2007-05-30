
/******************************************************************************
** Purpose : Create Roles and Accounts for Testing.
**
** Summary:
**
**   This script creates roles and accounts used for testing the SqlServerJobStore.
**
** Change History:
**
**   Date:    Author:  Bug #    Description:                           
**   -------- -------- ------   -----------------------------------------------
**   05/20/07 Jeff              Initial creation.
*******************************************************************************
** Copyright (C) 2007 Castle, All Rights Reserved
*******************************************************************************/

USE SchedulerTestDb
GO

/**
 * Create a role for users that have access to scheduling stored procedures.
 */
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = N'sql_dependency_subscriber' AND type = 'R')
BEGIN
	EXEC sp_droprolemember 'SchedulerRole', 'SchedulerTestUser'
	DROP ROLE SchedulerRole	
END
GO

CREATE ROLE SchedulerRole AUTHORIZATION dbo
GO 

/**
 * Create a test login.
 */
IF EXISTS (SELECT * FROM sys.server_principals WHERE name = N'SchedulerTestUser')
	DROP LOGIN SchedulerTestUser
GO

IF EXISTS (SELECT * FROM sys.database_principals WHERE name = N'SchedulerTestUser')
	DROP USER SchedulerTestUser
GO

CREATE LOGIN SchedulerTestUser WITH PASSWORD=N'test', DEFAULT_DATABASE=SchedulerTestDb, DEFAULT_LANGUAGE=us_english, CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

CREATE USER SchedulerTestUser FOR LOGIN SchedulerTestUser WITH DEFAULT_SCHEMA=dbo
GO

EXEC sp_addrolemember 'SchedulerRole', 'SchedulerTestUser'
GO
