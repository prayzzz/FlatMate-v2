--
-- Script
--

CREATE SCHEMA [Account]
GO

CREATE TABLE [Account].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[PasswordHash] [varchar](255) NOT NULL,
	[Salt] [varchar](255) NOT NULL,
	[UserName] [varbinary](255) NOT NULL,
  CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ( [Id] ASC )
)
GO

--
-- Migration
--

INSERT INTO [Infrastructure].[Migrations] ([FileName]) VALUES ('2017-04-29_18-10-35_UserTable')