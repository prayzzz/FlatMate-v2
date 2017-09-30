--
-- Script
--

SET IDENTITY_INSERT [Offers].[CompanyData] ON; 
GO

INSERT INTO [Offers].[CompanyData] ([Id], [Name]) VALUES (2, 'Penny')
GO

SET IDENTITY_INSERT [Offers].[CompanyData] OFF; 
GO


INSERT INTO [Offers].[Market] ([City],[CompanyId],[ExternalId],[Name],[PostalCode],[Street])
VALUES (' ', 2, ' ', 'Penny Deutschland', ' ', ' ')
GO


--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-09-30_15-13-37_add_penny');