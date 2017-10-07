--
-- Script
--

UPDATE [Offers].[Market] SET City = '', Street = '', PostalCode = '', ExternalId = '' WHERE ExternalId = ' '

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-10-07_17-50-33_fix_market_adresses');