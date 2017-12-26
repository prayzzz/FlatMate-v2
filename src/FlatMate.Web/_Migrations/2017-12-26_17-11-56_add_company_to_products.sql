--
-- Script
--

-- Offers
ALTER TABLE [Offers].[Offer]
  ADD [CompanyId] INT NULL;

ALTER TABLE [Offers].[Offer]
  WITH CHECK ADD CONSTRAINT [FK_Offer_Company] FOREIGN KEY ([CompanyId])
REFERENCES [Offers].[CompanyData] ([Id])
GO

ALTER TABLE [Offers].[Offer]
  CHECK CONSTRAINT [FK_Offer_Company]
GO

UPDATE [Offers].[Offer]
SET [CompanyId] = (SELECT TOP 1 [CompanyId]
                   FROM [Offers].[Market] m
                   WHERE [m].[Id] = [MarketId])

ALTER TABLE [Offers].[Offer]
  ALTER COLUMN [CompanyId] INT NOT NULL;

-- Product
ALTER TABLE [Offers].[Product]
  ADD [CompanyId] INT NULL;

ALTER TABLE [Offers].[Product]
  WITH CHECK ADD CONSTRAINT [FK_Product_Company] FOREIGN KEY ([CompanyId])
REFERENCES [Offers].[CompanyData] ([Id])
GO

ALTER TABLE [Offers].[Product]
  CHECK CONSTRAINT [FK_Product_Company]
GO

UPDATE [Offers].[Product]
SET [CompanyId] = (SELECT TOP 1 [CompanyId]
                   FROM [Offers].[Market] m
                   WHERE [m].[Id] = [MarketId])

ALTER TABLE [Offers].[Offer]
  ALTER COLUMN [CompanyId] INT NOT NULL;

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2017-12-26_17-11-56_add_company_to_products');