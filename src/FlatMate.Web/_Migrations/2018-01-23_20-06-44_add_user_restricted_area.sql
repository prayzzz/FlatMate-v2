--
-- Script
--

CREATE TABLE [Account].[UserRestrictedArea] (
  [Id]     [INT] IDENTITY (1, 1) NOT NULL,
  [UserId] [INT]                 NOT NULL,
  [Area]   [VARCHAR](255)        NOT NULL,
  CONSTRAINT [PK_UserRestrictedArea] PRIMARY KEY CLUSTERED ([Id] ASC)
)

ALTER TABLE [Account].[UserRestrictedArea]
  WITH CHECK ADD CONSTRAINT [FK_UserRestrictedArea_User_User] FOREIGN KEY ([UserId])
REFERENCES [Account].[User] ([Id])
GO

ALTER TABLE [Account].[UserRestrictedArea]
  CHECK CONSTRAINT [FK_UserRestrictedArea_User_User]
GO


--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2018-01-23_20-06-44_add_user_restricted_area');