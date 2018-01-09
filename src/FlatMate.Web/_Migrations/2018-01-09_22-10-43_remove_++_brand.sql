--
-- Script
--

UPDATE Offers.Product
SET Brand = ''
WHERE Brand LIKE '++%'

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2018-01-09_22-10-43_remove_++_brand');