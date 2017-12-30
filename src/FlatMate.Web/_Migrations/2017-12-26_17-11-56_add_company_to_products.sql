--
-- Script
--

-- Product
ALTER TABLE [Offers].[Product]
  ADD [CompanyId] INT NULL
GO


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
GO


ALTER TABLE [Offers].[Product]
  ALTER COLUMN [CompanyId] INT NOT NULL
GO


--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2017-12-26_17-11-56_add_company_to_products');