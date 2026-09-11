IF DB_ID('KafkaLab') IS NULL
BEGIN
    CREATE DATABASE KafkaLab;
END
GO

USE KafkaLab;
GO

IF OBJECT_ID('dbo.ProcessedEvents', 'U') IS NULL
BEGIN
CREATE TABLE dbo.ProcessedEvents
(
    EventId UNIQUEIDENTIFIER NOT NULL,
    ProcessedAt DATETIME2 NOT NULL,

    CONSTRAINT PK_ProcessedEvents
        PRIMARY KEY (EventId)
);
END
GO