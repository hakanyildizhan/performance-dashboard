IF EXISTS (select loginname from master.dbo.syslogins where name = N'IIS APPPOOL\PerformanceDashboard')
BEGIN
    BEGIN TRY
        DROP LOGIN [IIS APPPOOL\PerformanceDashboard];
    END TRY
    BEGIN CATCH
    END CATCH
END