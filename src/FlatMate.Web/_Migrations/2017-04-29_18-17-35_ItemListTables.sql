--
-- Script
--

CREATE SCHEMA [List]
GO

-- ItemList
CREATE TABLE [List].[ItemList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Description] [varchar](255) NULL,
	[IsPublic] [bit] NOT NULL,
	[LastEditorId] [int] NOT NULL,
	[Modified] [datetime] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[OwnerId] [int] NOT NULL,
  CONSTRAINT [PK_ItemList] PRIMARY KEY CLUSTERED ( [Id] ASC )
)
GO

ALTER TABLE [List].[ItemList]  WITH CHECK ADD  CONSTRAINT [FK_ItemList_User_LastEditor] FOREIGN KEY([LastEditorId])
REFERENCES [Account].[User] ([Id])
GO

ALTER TABLE [List].[ItemList] CHECK CONSTRAINT [FK_ItemList_User_LastEditor]
GO

ALTER TABLE [List].[ItemList]  WITH CHECK ADD  CONSTRAINT [FK_ItemList_User_Owner] FOREIGN KEY([OwnerId])
REFERENCES [Account].[User] ([Id])
GO

ALTER TABLE [List].[ItemList] CHECK CONSTRAINT [FK_ItemList_User_Owner]
GO

-- ItemGroup
CREATE TABLE [List].[ItemGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[ItemListId] [int] NOT NULL,
	[LastEditorId] [int] NOT NULL,
	[Modified] [datetime] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[SortIndex] [int] NOT NULL,
  CONSTRAINT [PK_ItemGroup] PRIMARY KEY CLUSTERED ( [Id] ASC )
) 
GO

ALTER TABLE [List].[ItemGroup]  WITH CHECK ADD  CONSTRAINT [FK_ItemGroup_ItemList] FOREIGN KEY([ItemListId])
REFERENCES [List].[ItemList] ([Id])
GO

ALTER TABLE [List].[ItemGroup] CHECK CONSTRAINT [FK_ItemGroup_ItemList]
GO

ALTER TABLE [List].[ItemGroup]  WITH CHECK ADD  CONSTRAINT [FK_ItemGroup_User_LastEditor] FOREIGN KEY([LastEditorId])
REFERENCES [Account].[User] ([Id])
GO

ALTER TABLE [List].[ItemGroup] CHECK CONSTRAINT [FK_ItemGroup_User_LastEditor]
GO

ALTER TABLE [List].[ItemGroup]  WITH CHECK ADD  CONSTRAINT [FK_ItemGroup_User_Owner] FOREIGN KEY([OwnerId])
REFERENCES [Account].[User] ([Id])
GO

ALTER TABLE [List].[ItemGroup] CHECK CONSTRAINT [FK_ItemGroup_User_Owner]
GO

-- Item
CREATE TABLE [List].[Item](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[ItemGroupId] [int] NULL,
	[ItemListId] [int] NOT NULL,
	[LastEditorId] [int] NOT NULL,
	[Modified] [datetime] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[SortIndex] [int] NOT NULL,
  CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED ( [Id] ASC )
)
GO

ALTER TABLE [List].[Item]  WITH CHECK ADD  CONSTRAINT [FK_Item_ItemGroup] FOREIGN KEY([ItemGroupId])
REFERENCES [List].[ItemGroup] ([Id])
GO

ALTER TABLE [List].[Item] CHECK CONSTRAINT [FK_Item_ItemGroup]
GO

ALTER TABLE [List].[Item]  WITH CHECK ADD  CONSTRAINT [FK_Item_ItemList] FOREIGN KEY([ItemListId])
REFERENCES [List].[ItemList] ([Id])
GO

ALTER TABLE [List].[Item] CHECK CONSTRAINT [FK_Item_ItemList]
GO

ALTER TABLE [List].[Item]  WITH CHECK ADD  CONSTRAINT [FK_Item_User_LastEditor] FOREIGN KEY([LastEditorId])
REFERENCES [Account].[User] ([Id])
GO

ALTER TABLE [List].[Item] CHECK CONSTRAINT [FK_Item_User_LastEditor]
GO

ALTER TABLE [List].[Item]  WITH CHECK ADD  CONSTRAINT [FK_Item_User_Owner] FOREIGN KEY([OwnerId])
REFERENCES [Account].[User] ([Id])
GO

ALTER TABLE [List].[Item] CHECK CONSTRAINT [FK_Item_User_Owner]
GO

--
-- Migration
--

INSERT INTO [Infrastructure].[Migrations] ([FileName]) VALUES ('2017-04-29_18-17-35_ItemListTables')