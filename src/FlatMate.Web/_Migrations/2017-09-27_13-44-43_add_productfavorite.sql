--
-- Script
--

CREATE TABLE [Offers].[ProductFavorite] (
	[Id] int NOT NULL IDENTITY (1, 1),	
	[ProductId] int NOT NULL,
	[UserId] int NOT NULL,
	CONSTRAINT [PK_ProductFavorite] PRIMARY KEY CLUSTERED ([Id])
)
GO

ALTER TABLE [Offers].[ProductFavorite] WITH CHECK ADD CONSTRAINT [FK_ProductFavorite_Product] FOREIGN KEY ([ProductId])
REFERENCES [Offers].[Product] ([Id])
GO

ALTER TABLE [Offers].[ProductFavorite] CHECK CONSTRAINT [FK_ProductFavorite_Product]
GO

ALTER TABLE [Offers].[ProductFavorite] WITH CHECK ADD CONSTRAINT [FK_ProductFavorite_User] FOREIGN KEY ([UserId])
REFERENCES [Account].[User] ([Id])
GO

ALTER TABLE [Offers].[ProductFavorite] CHECK CONSTRAINT [FK_ProductFavorite_User]
GO

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-09-27_13-44-43_add_productfavorite');