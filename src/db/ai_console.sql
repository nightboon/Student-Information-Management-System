-- Chat log table for the AI assistant / AI Console page.
-- One row per conversation (widget session); each exchange is appended to the
-- transcript. Re-run drops and recreates the table.
USE [StudentInformationManagementSystem];
GO

IF OBJECT_ID('dbo.CHAT_LOGS', 'U') IS NOT NULL
    DROP TABLE dbo.CHAT_LOGS;
GO

CREATE TABLE dbo.CHAT_LOGS (
    chat_log_id     INT IDENTITY (1, 1) PRIMARY KEY,
    conversation_id UNIQUEIDENTIFIER NOT NULL UNIQUE,
    user_id         INT              NOT NULL,
    transcript      NVARCHAR (MAX)   NOT NULL,
    turns           INT              NOT NULL DEFAULT 1,
    tools_used      NVARCHAR (400)   NULL,
    duration_ms     INT              NOT NULL DEFAULT 0,
    created_at      DATETIME         NOT NULL DEFAULT GETDATE(),
    updated_at      DATETIME         NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_CHAT_LOGS_USERS FOREIGN KEY (user_id) REFERENCES dbo.USERS (user_id)
);
GO
