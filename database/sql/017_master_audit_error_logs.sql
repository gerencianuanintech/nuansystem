IF OBJECT_ID(N'dbo.AuditErrorLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditErrorLogs
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditErrorLogs PRIMARY KEY,
        [Source] nvarchar(30) NOT NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        CompanyCode nvarchar(50) NULL,
        ModuleKey nvarchar(120) NULL,
        FormName nvarchar(180) NULL,
        ActionName nvarchar(120) NULL,
        HttpMethod nvarchar(12) NULL,
        [Path] nvarchar(500) NULL,
        QueryString nvarchar(1000) NULL,
        StatusCode int NULL,
        ErrorMessage nvarchar(2000) NOT NULL,
        ExceptionType nvarchar(300) NULL,
        StackTrace nvarchar(max) NULL,
        TraceId nvarchar(120) NULL,
        IpAddress nvarchar(64) NULL,
        MachineName nvarchar(120) NULL,
        UserAgent nvarchar(500) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditErrorLogs_CreatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_AuditErrorLogs_CreatedAt ON dbo.AuditErrorLogs (CreatedAt DESC);
    CREATE INDEX IX_AuditErrorLogs_UserId ON dbo.AuditErrorLogs (UserId, CreatedAt DESC);
    CREATE INDEX IX_AuditErrorLogs_Source_CreatedAt ON dbo.AuditErrorLogs ([Source], CreatedAt DESC);
END;
GO
