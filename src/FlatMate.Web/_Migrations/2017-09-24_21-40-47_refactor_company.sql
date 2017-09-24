--
-- Script
--


EXEC sp_rename '[Offers].[Company]', 'CompanyData';
GO


--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-09-24_21-40-47_refactor_company');