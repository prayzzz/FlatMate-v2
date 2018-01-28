--
-- Script
--


ALTER TABLE flatmate.Account.[User]
  ADD IsActivated BIT
GO

UPDATE flatmate.Account.[User]
SET IsActivated = 1
GO

ALTER TABLE flatmate.Account.[User]
  ALTER COLUMN IsActivated BIT NOT NULL
GO


--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2018-01-28_15-37-07_add_activated_flag');