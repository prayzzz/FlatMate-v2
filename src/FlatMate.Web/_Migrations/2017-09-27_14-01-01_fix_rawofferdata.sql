--
-- Script
--

ALTER TABLE [Offers].[RawOfferData] DROP CONSTRAINT [PK_RawOfferData]
GO

ALTER TABLE [Offers].[RawOfferData] ADD CONSTRAINT [PK_RawOfferData] PRIMARY KEY CLUSTERED ([Id]) 
GO

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-09-27_14-01-01_fix_rawofferdata');