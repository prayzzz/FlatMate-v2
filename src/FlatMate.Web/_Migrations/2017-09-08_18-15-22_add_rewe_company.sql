--
-- Script
--


SET IDENTITY_INSERT [Offers].[Company] ON
GO

IF (SELECT Count(*) FROM [Offers].[Company] WHERE [Id] = 1) = 0
BEGIN
	INSERT INTO [Offers].[Company] ([Id], [Name]) VALUES (1, 'REWE')
END
GO

SET IDENTITY_INSERT [Offers].[Company] OFF
GO

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-09-08_18-15-22_add_rewe_company');