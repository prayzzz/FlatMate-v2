--
-- Script
--


ALTER TABLE [Offers].[PriceHistory]
  ADD [MarketId] INT NULL
GO


ALTER TABLE [Offers].[PriceHistory]
  WITH CHECK ADD CONSTRAINT [FK_PriceHistory_Market] FOREIGN KEY ([MarketId])
REFERENCES [Offers].[Market] ([Id])
GO


ALTER TABLE [Offers].[PriceHistory]
  CHECK CONSTRAINT [FK_PriceHistory_Market]
GO


UPDATE [Offers].[PriceHistory]
SET [MarketId] = (SELECT TOP 1 p.[MarketId]
                  FROM [Offers].[Product] p
                  WHERE [p].[Id] = [ProductId])
GO


ALTER TABLE [Offers].[PriceHistory]
  ALTER COLUMN [MarketId] INT NOT NULL
GO


--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2017-12-27_09-11-38_pricehistory_per_product_and_market');