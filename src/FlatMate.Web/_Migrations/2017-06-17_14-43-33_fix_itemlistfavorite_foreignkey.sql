--
-- Script
--

ALTER TABLE [List].[ItemListFavorite] DROP CONSTRAINT [FK_ItemListFavorite_Item]
GO

ALTER TABLE [List].[ItemListFavorite]  WITH CHECK ADD  CONSTRAINT [FK_ItemListFavorite_ItemList] FOREIGN KEY([ItemListId])
REFERENCES [List].ItemList ([Id])
GO

ALTER TABLE [List].[ItemListFavorite] CHECK CONSTRAINT [FK_ItemListFavorite_ItemList]
GO

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-06-17_14-43-33_fix_itemlistfavorite_foreignkey');