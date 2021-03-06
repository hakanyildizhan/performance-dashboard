IF NOT EXISTS (select loginname from master.dbo.syslogins where name = N'IIS APPPOOL\PerformanceDashboard')
BEGIN
    CREATE LOGIN [IIS APPPOOL\PerformanceDashboard] FROM WINDOWS WITH DEFAULT_DATABASE=[master];
	USE [PerformanceDashboard]
	CREATE USER [IIS APPPOOL\PerformanceDashboard] FOR LOGIN [IIS APPPOOL\PerformanceDashboard] WITH DEFAULT_SCHEMA=[dbo];
	ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\PerformanceDashboard];
	ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\PerformanceDashboard];
END
ELSE 
BEGIN
	IF NOT EXISTS (SELECT [name] FROM [sys].[database_principals]
       WHERE [type] = N'S' AND [name] = N'IIS APPPOOL\PerformanceDashboard')
	BEGIN
		USE [PerformanceDashboard]
		CREATE USER [IIS APPPOOL\PerformanceDashboard] FOR LOGIN [IIS APPPOOL\PerformanceDashboard] WITH DEFAULT_SCHEMA=[dbo];
		ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\PerformanceDashboard];
		ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\PerformanceDashboard];
	END
END