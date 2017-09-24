--
-- Script
--

CREATE TABLE [Offers].[RawOfferData] (
	[Id] int NOT NULL IDENTITY (1, 1),	
	[Created] datetime NOT NULL,
	[Data] text NOT NULL,
	[Hash] varchar(32) NOT NULL,
	[CompanyId] int NOT NULL
	CONSTRAINT [PK_RawOfferData] PRIMARY KEY CLUSTERED
)
GO

ALTER TABLE [Offers].[RawOfferData] WITH CHECK ADD CONSTRAINT [FK_RawOfferData_Company] FOREIGN KEY ([CompanyId])
REFERENCES [Offers].[Company] ([Id])
GO

ALTER TABLE [Offers].[RawOfferData] CHECK CONSTRAINT [FK_RawOfferData_Company]
GO

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-09-24_20-13-40_add_offer_data_response_table');