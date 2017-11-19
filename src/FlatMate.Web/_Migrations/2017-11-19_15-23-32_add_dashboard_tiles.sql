--
-- Script
--

CREATE TABLE [Account].[UserDashboardTile] (
  [Id]            [INT] IDENTITY (1, 1) NOT NULL,
  [UserId]        [INT]                 NOT NULL,
  [DashboardTile] [VARCHAR](255)        NOT NULL,
  [Parameter]     [TEXT]                NOT NULL,
  CONSTRAINT [PK_UserDashBoardTiles] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

ALTER TABLE [Account].[UserDashboardTile]
  WITH CHECK ADD CONSTRAINT [FK_UserDashboardTile_User_User] FOREIGN KEY ([UserId])
REFERENCES [Account].[User] ([Id])
GO

ALTER TABLE [Account].[UserDashboardTile]
  CHECK CONSTRAINT [FK_UserDashboardTile_User_User]
GO

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2017-11-19_15-23-32_add_dashboard_tiles');