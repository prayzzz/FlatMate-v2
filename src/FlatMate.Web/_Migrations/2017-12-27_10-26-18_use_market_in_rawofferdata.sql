--
-- Script
--

ALTER TABLE [Offers].[RawOfferData]
  ADD [MarketId] INT NULL
GO


ALTER TABLE [Offers].[RawOfferData]
  WITH CHECK ADD CONSTRAINT [FK_RawOfferData_Market] FOREIGN KEY ([MarketId])
REFERENCES [Offers].[Market] ([Id])
GO


ALTER TABLE [Offers].[RawOfferData]
  CHECK CONSTRAINT [FK_RawOfferData_Market]
GO


UPDATE [Offers].[RawOfferData]
SET [MarketId] = (SELECT TOP 1 [Market].[Id]
                  FROM [Offers].[Market]
                  WHERE [Market].[CompanyId] = [RawOfferData].[CompanyId])
GO


ALTER TABLE [Offers].[RawOfferData]
  ALTER COLUMN [MarketId] INT NOT NULL
GO


ALTER TABLE [Offers].[RawOfferData]
  DROP CONSTRAINT [FK_RawOfferData_Company]
GO


ALTER TABLE [Offers].[RawOfferData]
  DROP COLUMN [CompanyId]
GO


--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2017-12-27_10-26-18_use_market_in_rawofferdata');