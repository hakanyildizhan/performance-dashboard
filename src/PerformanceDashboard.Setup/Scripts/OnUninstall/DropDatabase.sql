IF DB_ID('PerformanceDashboard') IS NOT NULL
BEGIN
    BEGIN TRY
        USE master;
	    ALTER DATABASE PerformanceDashboard SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	    DROP DATABASE PerformanceDashboard;
    END TRY
    BEGIN CATCH
        ALTER DATABASE PerformanceDashboard SET multi_user WITH ROLLBACK IMMEDIATE;
    END CATCH
END