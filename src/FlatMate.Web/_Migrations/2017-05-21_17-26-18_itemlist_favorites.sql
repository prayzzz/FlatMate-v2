--
-- Script
--

CREATE TABLE [List].[ItemListFavorite](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemListId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
  CONSTRAINT [PK_ItemListFavorite] PRIMARY KEY CLUSTERED ( [Id] ASC )
)
GO

ALTER TABLE [List].[ItemListFavorite]  WITH CHECK ADD  CONSTRAINT [FK_ItemListFavorite_Item] FOREIGN KEY([ItemListId])
REFERENCES [List].[Item] ([Id])
GO

ALTER TABLE [List].[ItemListFavorite] CHECK CONSTRAINT [FK_ItemListFavorite_Item]
GO

ALTER TABLE [List].[ItemListFavorite]  WITH CHECK ADD  CONSTRAINT [FK_ItemListFavorite_User] FOREIGN KEY([UserId])
REFERENCES [Account].[User] ([Id])
GO

ALTER TABLE [List].[ItemListFavorite] CHECK CONSTRAINT [FK_ItemListFavorite_User]
GO


--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-05-21_17-26-18_itemlist_favorites');