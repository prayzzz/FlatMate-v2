--
-- Script
--

-- https://docs.microsoft.com/en-us/sql/t-sql/data-types/ntext-text-and-image-transact-sql?view=sql-server-2017
-- IMPORTANT! ntext, text, and image data types will be removed in a future version of SQL Server.
-- Avoid using these data types in new development work, and plan to modify applications that currently use them.
-- Use nvarchar(max), varchar(max), and varbinary(max) instead.

ALTER TABLE Offers.Offer
  ALTER COLUMN ImageUrl NVARCHAR(max) null

ALTER TABLE Offers.Product
  ALTER COLUMN [Description] NVARCHAR(max) null
ALTER TABLE Offers.Product
  ALTER COLUMN ImageUrl NVARCHAR(max) null

ALTER TABLE Offers.RawOfferData
  ALTER COLUMN Data NVARCHAR(max) null

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2019-07-06_21-43-33_text_to_varchar');