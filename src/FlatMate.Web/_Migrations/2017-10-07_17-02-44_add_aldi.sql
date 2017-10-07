--
-- Script
--


SET IDENTITY_INSERT [Offers].[CompanyData] ON; 
GO

INSERT INTO [Offers].[CompanyData] ([Id], [Name]) VALUES (5, 'Aldi Nord')
GO

SET IDENTITY_INSERT [Offers].[CompanyData] OFF; 
GO


INSERT INTO [Offers].[Market] ([City],[CompanyId],[ExternalId],[Name],[PostalCode],[Street])
VALUES (' ', 5, ' ', 'Aldi Nord Deutschland', ' ', ' ')
GO

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-10-07_17-02-44_add_aldi');