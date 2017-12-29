--
-- Script
--


ALTER TABLE [Offers].[Product]
  DROP COLUMN [ExternalId]
GO


ALTER TABLE [Offers].[Product]
  DROP COLUMN [Price]
GO


--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2017-12-27_10-09-17_remove_external_product_id');