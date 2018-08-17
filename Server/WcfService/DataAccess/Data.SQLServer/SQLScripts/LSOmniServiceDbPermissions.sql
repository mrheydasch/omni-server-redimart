-----------------------------
 -----  create login and user
-----------------------------
USE master
GO

BEGIN TRY
    IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'LSOmniUser')
    begin
        CREATE LOGIN LSOmniUser WITH PASSWORD=N'LSOmniUser',  DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF;
    end
END TRY 
BEGIN CATCH
     Begin;
      RAISERROR ('Could not create login LSOmniUser . Make sure you have SysAdmin permisssion on this sql server.', -- Message text.
                   16, -- Severity.
                   1 -- State.
                   );
     End 
END CATCH; 
GO

USE [THEDATABASE]
GO 

BEGIN TRY
    -- drop the user to the internal id match the login
    IF EXISTS (SELECT * FROM sys.database_principals WHERE name = N'LSOmniUser')
    begin	
        DROP USER LSOmniUser;
    end
    CREATE USER LSOmniUser FOR LOGIN LSOmniUser WITH DEFAULT_SCHEMA=[dbo];

    EXEC sp_addrolemember N'db_datawriter', N'LSOmniUser';
    EXEC sp_addrolemember N'db_datareader', N'LSOmniUser';
    GRANT EXECUTE TO  LSOmniUser;
END TRY 
BEGIN CATCH
     Begin;
      RAISERROR ('Could not create user LSOmniUser  user. Make sure you have SysAdmin permisssion on this sql server.', -- Message text.
                   16, -- Severity.
                   1 -- State.
                   );
     End 
END CATCH; 
GO 
